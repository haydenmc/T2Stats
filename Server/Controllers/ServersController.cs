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
            return Ok(db.Servers);
        }

        [HttpGet]
        [Route("{serverIpAddress}/{serverPort}")]
        public IActionResult GetMatches(string serverIpAddress, string serverPort)
        {
            return Ok(db.Matches);
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
            // Find the match
            var match = db.Matches.Where(m =>
                m.StartTime > approxMatchStartTime.AddSeconds(-MatchStartTimeToleranceSeconds) &&
                m.StartTime < approxMatchStartTime.AddSeconds(MatchStartTimeToleranceSeconds)
            ).OrderBy(m => Math.Abs(m.StartTime.Subtract(approxMatchStartTime).TotalSeconds)).FirstOrDefault();
            // Get all kill events
            var matchKillEvents = db.KillEvents.Where(k => k.MatchId == match.MatchId);
            return Ok(matchKillEvents);
        }
    }
}