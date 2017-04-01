using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T2Stats.Models;

namespace T2Stats.Controllers
{
    [Route("Servers")]
    public class ServersController : Controller
    {
        private const int MatchStartTimeToleranceSeconds = 15;
        private const int EventTimeToleranceSeconds = 4;

        private readonly ApplicationDbContext db;

        public ServersController(ApplicationDbContext db) : base()
        {
            this.db = db;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetServers()
        {
            var servers = db.Events
                .Select(e => new { e.ServerIpAddress, e.ServerPort }).Distinct()
                .Select(e => new
                {
                    ServerName = db.Events
                        .Where(ev => ev.ServerIpAddress == e.ServerIpAddress && ev.ServerPort == e.ServerPort)
                        .OrderByDescending(ev => ev.MatchStartTime).OrderByDescending(ev => ev.MatchTime)
                        .FirstOrDefault().ServerName,
                    e.ServerIpAddress,
                    e.ServerPort
                });
            return Ok(servers);
        }

        [HttpGet]
        [Route("{serverIpAddress}/{serverPort}")]
        public IActionResult GetMatches(string serverIpAddress, string serverPort)
        {
            // Unique fields that denote matches
            var t1 = db.Events
                .Select(e => new { e.MatchGameType, e.MatchMapName, e.MatchStartTime, e.ServerIpAddress, e.ServerPort} )
                .Distinct()
                .Where(e => e.ServerIpAddress == serverIpAddress && e.ServerPort == int.Parse(serverPort));
                
            // For each row...
            var matches = t1
                .Select(te => new
                {
                    // Find all within x # seconds
                    matches = t1.Where(tee =>
                        te.MatchStartTime.Subtract(tee.MatchStartTime).TotalSeconds < MatchStartTimeToleranceSeconds &&
                        te.MatchStartTime.Subtract(tee.MatchStartTime).TotalSeconds > -MatchStartTimeToleranceSeconds
                    ).OrderBy(tee => tee.MatchStartTime),
                    te.MatchStartTime,
                }
                )
                .Where(te =>
                    te.matches.FirstOrDefault().MatchStartTime >= te.MatchStartTime
                ).Select(te => new
                {
                    te.matches.FirstOrDefault().MatchGameType,
                    te.matches.FirstOrDefault().MatchMapName,
                    te.MatchStartTime,
                    te.matches.FirstOrDefault().ServerIpAddress,
                    te.matches.FirstOrDefault().ServerPort
                });
            return Ok(matches);
        }

        [HttpGet]
        [Route("{serverIpAddress}/{serverPort}/{approxMatchStartTime:datetime}")]
        public IActionResult GetMatchEvents(string serverIpAddress, string serverPort, DateTimeOffset approxMatchStartTime)
        {
            return Ok("Coming soon...");
        }

        [HttpGet]
        [Route("{serverIpAddress}/{serverPort}/{approxMatchStartTimeStr}/Kills")]
        public IActionResult GetMatchKills(string serverIpAddress, string serverPort, string approxMatchStartTimeStr)
        {
            DateTimeOffset approxMatchStartTime;
            string[] patterns = { "yyyy-MM-dd'T'HH:mm:ss.FFFK", "yyyy-MM-dd'T'HH:mm:ssK" };
            if (!DateTimeOffset.TryParseExact(approxMatchStartTimeStr, patterns, CultureInfo.InvariantCulture, DateTimeStyles.None, out approxMatchStartTime))
            {
                return BadRequest("Could not parse match start time parameter.");
            }
            // Get all kill events
            var matchKillEvents = db.KillEvents
                .Where(e =>
                    e.ServerIpAddress == serverIpAddress &&
                    e.ServerPort == int.Parse(serverPort) &&
                    e.MatchStartTime > approxMatchStartTime.AddSeconds(-MatchStartTimeToleranceSeconds) &&
                    e.MatchStartTime < approxMatchStartTime.AddSeconds(MatchStartTimeToleranceSeconds)
                );
            // Naive approach. Maybe this can be accomplished in SQL or something later...
            var killEventGroups = new Dictionary<long, List<List<KillEvent>>>();
            foreach (var killEvent in matchKillEvents)
            {
                // Look for nearby event group matches
                List<KillEvent> eventGroup = null;
                for (int i = 0; i < EventTimeToleranceSeconds; i++)
                {
                    // Check forward and backward
                    for (int j = 1; j != -1; j = -1)
                    {
                        var keyOffset = i * j;
                        // Find groups with the right timestamp
                        if (killEventGroups.ContainsKey((long)killEvent.MatchTime.TotalSeconds + keyOffset))
                        {
                            // Check each event group for a match
                            foreach (var eg in killEventGroups[(long)killEvent.MatchTime.TotalSeconds + keyOffset])
                            {
                                if (IsKillMatch(eg.First(), killEvent))
                                {
                                    eventGroup = eg;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (eventGroup == null)
                {
                    // Make a new EventGroup
                    eventGroup = new List<KillEvent>();
                    // Add it to the proper place
                    if (!killEventGroups.ContainsKey((long)killEvent.MatchTime.TotalSeconds))
                    {
                        killEventGroups[(long)killEvent.MatchTime.TotalSeconds] = new List<List<KillEvent>>();
                    }
                    killEventGroups[(long)killEvent.MatchTime.TotalSeconds].Add(eventGroup);
                }
                eventGroup.Add(killEvent);
            }
            return Ok(killEventGroups.Values.SelectMany(v => v));
        }

        // Used to determine if one KillEvent is an approximate match to another, disregarding time tolerance and reporter.
        private bool IsKillMatch(KillEvent killEvent, KillEvent otherEvent)
        {
            return (
                killEvent.KillType == otherEvent.KillType &&
                killEvent.Weapon == otherEvent.Weapon &&
                killEvent.KillerTribesGuid == otherEvent.KillerTribesGuid &&
                killEvent.VictimTribesGuid == otherEvent.VictimTribesGuid
            );
        }
    }
}