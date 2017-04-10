using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using T2Stats.Models;

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
            public List<(string reporterGuid, string reporterName)> Reporters;
            public Timer Timer;
        }

        /// <summary>
        /// Timeout before a pending event is committed.
        /// </summary>
        private const int EventCommitTimeoutSeconds = 5;

        /// <summary>
        /// List of events pending ingestion.
        /// </summary>
        private List<PendingEvent> pendingEvents = new List<PendingEvent>();

        /// <summary>
        /// Reference to the database.
        /// </summary>
        private ApplicationDbContext db;

        /// <summary>
        /// Instantiates EventIngestionService with reference to database.
        /// </summary>
        /// <param name="db"></param>
        public EventIngestionService(ApplicationDbContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Records a new event
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="reporterGuid"></param>
        /// <param name="reporterName"></param>
        public void RecordEvent(Event ev, string reporterGuid, string reporterName)
        {
            // See if there's a metching event in our pending store.
            for (var i = 0; i < pendingEvents.Count; i++)
            {
                if (ev.EventCompareTo(pendingEvents[i].Ev))
                {
                    pendingEvents[i].Reporters.Add((reporterGuid: reporterGuid, reporterName: reporterName));
                    return;
                }
            }
            // Otherwise add a new one and set the clock
            var pendingEv = new PendingEvent()
            {
                Ev = ev,
                Reporters = new List<(string reporterGuid, string reporterName)>()
                {
                    (reporterGuid: reporterGuid, reporterName: reporterName)
                }
            };
            pendingEvents.Add(pendingEv);
            pendingEv.Timer = new Timer((timerState) => {
                CommitEvent(pendingEv);
            }, null, EventCommitTimeoutSeconds * 1000, System.Threading.Timeout.Infinite);
        }

        private void CommitEvent(PendingEvent pendingEvent)
        {
            pendingEvents.Remove(pendingEvent);
            // Look up/add each reporter
            for (var i = 0; i < pendingEvent.Reporters.Count; i++)
            {
                Player player = db.Players.FirstOrDefault(p => p.TribesGuid == pendingEvent.Reporters[i].reporterGuid);
                if (player == null)
                {
                    player = new Player()
                    {
                        PlayerId = Guid.NewGuid(),
                        TribesGuid = pendingEvent.Reporters[i].reporterGuid,
                        Name = pendingEvent.Reporters[i].reporterName
                    };
                    db.Players.Add(player);
                }
                pendingEvent.Ev.EventReports.Add(new EventReporter()
                {
                    Player = player,
                    Event = pendingEvent.Ev
                });
            }
            
            if (pendingEvent.Ev is KillEvent)
            {
                var pendingKillEvent = pendingEvent.Ev as KillEvent;
                // Look up victim, killer
                pendingKillEvent.Killer = db.Players.FirstOrDefault(p => p.TribesGuid == pendingKillEvent.KillerTribesGuid);
                if (pendingKillEvent.Killer == null)
                {
                    pendingKillEvent.Killer = new Player()
                    {
                        PlayerId = Guid.NewGuid(),
                        TribesGuid = pendingKillEvent.KillerTribesGuid,
                        Name = pendingKillEvent.KillerName
                    };
                    db.Players.Add(pendingKillEvent.Killer);
                }
                pendingKillEvent.Victim = db.Players.FirstOrDefault(p => p.TribesGuid == pendingKillEvent.VictimTribesGuid);
                if (pendingKillEvent.Victim == null)
                {
                    pendingKillEvent.Victim = new Player()
                    {
                        PlayerId = Guid.NewGuid(),
                        TribesGuid = pendingKillEvent.VictimTribesGuid,
                        Name = pendingKillEvent.VictimName
                    };
                    db.Players.Add(pendingKillEvent.Victim);
                }
                db.KillEvents.Add(pendingKillEvent);
            }
            else
            {
                db.Events.Add(pendingEvent.Ev);
            }
            db.SaveChanges();
        }
    }
}