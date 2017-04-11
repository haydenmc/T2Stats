using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using T2Stats.Models;
using T2Stats.Models.BindingModels;

namespace T2Stats.Services
{
    /// <summary>
    /// The EventIngestionService acts as a buffer between incoming events in the database.
    /// It correlates incoming events and flattens them before committing them.
    /// </summary>
    public class EventIngestionService
    {
        /// <summary>
        /// Transient class that serves to store pending event information.
        /// </summary>
        private class PendingEvent
        {
            public Event Ev;
            public List<PlayerBindingModel> Reporters;
            public MatchBindingModel Match;
            public Timer Timer;
        }

        /// <summary>
        /// Logging object
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Timeout before a pending event is committed.
        /// </summary>
        private const int EventCommitTimeoutSeconds = 5;

        /// <summary>
        /// Tolerance for match time offsets to be considered the same match.
        /// </summary>
        private const int MatchStartTimeToleranceSeconds = 15;

        /// <summary>
        /// List of events pending ingestion.
        /// </summary>
        private List<PendingEvent> pendingEvents = new List<PendingEvent>();

        /// <summary>
        /// Reference to the database.
        /// </summary>
        private ApplicationDbContext db;

        /// <summary>
        /// Locking object for event record method.
        /// </summary>
        private Object recordLock = new Object();

        /// <summary>
        /// Locking object for event commit method.
        /// </summary>
        private Object commitLock = new Object();

        /// <summary>
        /// Instantiates EventIngestionService with reference to database.
        /// </summary>
        /// <param name="db"></param>
        public EventIngestionService(ApplicationDbContext db, ILogger<EventIngestionService> logger)
        {
            this.db = db;
            this.logger = logger;
            logger.LogInformation("Started event ingestion service.");
        }

        /// <summary>
        /// Records a new event
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="reporterGuid"></param>
        /// <param name="reporterName"></param>
        public void RecordEvent(Event ev, PlayerBindingModel reporter, MatchBindingModel match)
        {
            lock (recordLock)
            {
                logger.LogInformation($"Recording new {ev.GetType().Name}...");
                // See if there's a metching event in our pending store.
                for (var i = 0; i < pendingEvents.Count; i++)
                {
                    if (IsSameMatch(match, pendingEvents[i].Match) && ev.EventCompareTo(pendingEvents[i].Ev))
                    {
                        pendingEvents[i].Reporters.Add(reporter);
                        logger.LogInformation($"\t{ev.GetType().Name} matched existing pending event.");
                        return;
                    }
                }
                // Otherwise add a new one and set the clock
                var pendingEv = new PendingEvent()
                {
                    Ev = ev,
                    Reporters = new List<PlayerBindingModel>()
                    {
                        reporter
                    },
                    Match = match
                };
                pendingEvents.Add(pendingEv);
                pendingEv.Timer = new Timer((timerState) => {
                    CommitEvent(pendingEv);
                }, null, EventCommitTimeoutSeconds * 1000, System.Threading.Timeout.Infinite);
                logger.LogInformation($"\t{ev.GetType().Name} recorded as new event.");
            }
        }

        /// <summary>
        /// Commits a pending event to the database and clears it from memory.
        /// </summary>
        /// <param name="pendingEvent"></param>
        private void CommitEvent(PendingEvent pendingEvent)
        {
            lock (commitLock) // client-side sync to avoid SQL perf issues
            {
                logger.LogInformation($"Committing {pendingEvent.Ev.GetType().Name} to database.");
                pendingEvents.Remove(pendingEvent);
                // Look up/add each reporter
                for (var i = 0; i < pendingEvent.Reporters.Count; i++)
                {
                    Player player = db.Players.FirstOrDefault(p => p.TribesGuid == pendingEvent.Reporters[i].TribesGuid);
                    if (player == null)
                    {
                        player = new Player()
                        {
                            PlayerId = Guid.NewGuid(),
                            TribesGuid = pendingEvent.Reporters[i].TribesGuid,
                            Name = pendingEvent.Reporters[i].Name
                        };
                        db.Players.Add(player);
                        logger.LogInformation($"\t New player reporter: {player.Name}/{player.TribesGuid}");
                    }
                    pendingEvent.Ev.EventReports.Add(new EventReporter()
                    {
                        Player = player,
                        Event = pendingEvent.Ev
                    });
                }
                // Look up server
                Models.Server dbServer = db.Servers.SingleOrDefault(s =>
                    s.IpAddress == pendingEvent.Match.Server.IpAddress &&
                    s.Port == pendingEvent.Match.Server.Port
                );
                if (dbServer == null)
                {
                    dbServer = new Models.Server()
                    {
                        ServerId = Guid.NewGuid(),
                        IpAddress = pendingEvent.Match.Server.IpAddress,
                        Port = pendingEvent.Match.Server.Port,
                        Name = pendingEvent.Match.Server.Name
                    };
                    db.Servers.Add(dbServer);
                    logger.LogInformation($"\t New server: {dbServer.Name}/{dbServer.IpAddress}:{dbServer.Port}");
                }
                // Look up match
                Match dbMatch = db.Matches.SingleOrDefault(m =>
                    m.Server.IpAddress == pendingEvent.Match.Server.IpAddress &&
                    m.Server.Port == pendingEvent.Match.Server.Port &&
                    m.StartTime.Subtract(pendingEvent.Match.StartTime).TotalSeconds < MatchStartTimeToleranceSeconds &&
                    m.StartTime.Subtract(pendingEvent.Match.StartTime).TotalSeconds > -MatchStartTimeToleranceSeconds &&
                    m.MapName == pendingEvent.Match.Map.Name &&
                    m.GameType == pendingEvent.Match.GameType
                );
                if (dbMatch == null)
                {
                    dbMatch = new Match()
                    {
                        MatchId = Guid.NewGuid(),
                        ServerId = dbServer.ServerId,
                        MapName = pendingEvent.Match.Map.Name,
                        GameType = pendingEvent.Match.GameType,
                        StartTime = pendingEvent.Match.StartTime,
                        Duration = TimeSpan.FromMinutes(pendingEvent.Match.TimeLimitMinutes)
                    };
                    db.Matches.Add(dbMatch);
                    logger.LogInformation($"\t New match: {dbMatch.GameType}/{dbMatch.MapName} @ {dbMatch.StartTime}");
                }

                // Set event navigational properties
                pendingEvent.Ev.MatchId = dbMatch.MatchId;

                if (pendingEvent.Ev is KillEvent)
                {
                    var pendingKillEvent = pendingEvent.Ev as KillEvent;
                    // Look up victim, killer
                    pendingKillEvent.Killer = db.Players.Local.FirstOrDefault(p => p.TribesGuid == pendingKillEvent.KillerTribesGuid);
                    if (pendingKillEvent.Killer == null)
                    {
                        pendingKillEvent.Killer = new Player()
                        {
                            PlayerId = Guid.NewGuid(),
                            TribesGuid = pendingKillEvent.KillerTribesGuid,
                            Name = pendingKillEvent.KillerName
                        };
                        db.Players.Add(pendingKillEvent.Killer);
                        logger.LogInformation($"\t New player killer {pendingKillEvent.Killer.Name}/{pendingKillEvent.Killer.TribesGuid}.");
                    }
                    pendingKillEvent.Victim = db.Players.Local.FirstOrDefault(p => p.TribesGuid == pendingKillEvent.VictimTribesGuid);
                    if (pendingKillEvent.Victim == null)
                    {
                        pendingKillEvent.Victim = new Player()
                        {
                            PlayerId = Guid.NewGuid(),
                            TribesGuid = pendingKillEvent.VictimTribesGuid,
                            Name = pendingKillEvent.VictimName
                        };
                        db.Players.Add(pendingKillEvent.Victim);
                        logger.LogInformation($"\t New player victim {pendingKillEvent.Victim.Name}/{pendingKillEvent.Victim.TribesGuid}.");
                    }
                    db.KillEvents.Add(pendingKillEvent);
                    logger.LogInformation($"\t Added as Kill Event.");
                }
                else
                {
                    db.Events.Add(pendingEvent.Ev);
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Given two MatchBindingModel instances, determine whether these may refer to the same Match.
        /// </summary>
        /// <param name="matchOne">First to compare</param>
        /// <param name="matchTwo">Second to compare</param>
        /// <returns>True if instances may refer to the same Match, otherwise false</returns>
        private bool IsSameMatch(MatchBindingModel matchOne, MatchBindingModel matchTwo)
        {
            return (
                matchOne.Server.IpAddress == matchTwo.Server.IpAddress &&
                matchOne.Server.Port == matchTwo.Server.Port &&
                matchOne.Map.Name == matchTwo.Map.Name &&
                matchOne.GameType == matchTwo.GameType &&
                matchOne.StartTime.Subtract(matchTwo.StartTime).TotalSeconds < MatchStartTimeToleranceSeconds &&
                matchOne.StartTime.Subtract(matchTwo.StartTime).TotalSeconds > -MatchStartTimeToleranceSeconds
            );
        }
    }
}