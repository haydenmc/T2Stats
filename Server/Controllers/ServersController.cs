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
    }
}