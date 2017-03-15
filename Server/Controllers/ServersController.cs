using System;
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
            // Unique fields that denote events
            var t1 = db.KillEvents
                .Select(e => new {
                    e.ReporterTribesGuid,
                    e.ServerIpAddress,
                    e.ServerPort,
                    e.MatchStartTime,
                    e.MatchTime,
                    e.VictimTribesGuid,
                    e.KillerTribesGuid,
                    e.Weapon,
                    e.KillType
                })
                .Distinct()
                .Where(e =>
                    e.ServerIpAddress == serverIpAddress &&
                    e.ServerPort == int.Parse(serverPort) &&
                    e.MatchStartTime > approxMatchStartTime.AddSeconds(-MatchStartTimeToleranceSeconds) &&
                    e.MatchStartTime < approxMatchStartTime.AddSeconds(MatchStartTimeToleranceSeconds)
                );
            // For each row...
            var matches = t1
                .Select(te => new
                {
                    // Find all within x # seconds
                    matches = t1.Where(tee =>
                        te.MatchTime.Subtract(tee.MatchTime).TotalSeconds < EventTimeToleranceSeconds &&
                        te.MatchTime.Subtract(tee.MatchTime).TotalSeconds > -EventTimeToleranceSeconds
                    ).OrderBy(tee => tee.MatchTime),
                    te.MatchTime,
                }
                )
                .Where(te =>
                    te.matches.FirstOrDefault().MatchTime >= te.MatchTime
                ).Select(te => new
                {
                    te.matches.FirstOrDefault().ReporterTribesGuid,
                    te.matches.FirstOrDefault().MatchTime,
                    te.matches.FirstOrDefault().VictimTribesGuid,
                    te.matches.FirstOrDefault().KillerTribesGuid,
                    te.matches.FirstOrDefault().Weapon,
                    te.matches.FirstOrDefault().KillType
                });
            return Ok(matches);
        }
    }
}