using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T2Stats.Models;
using T2Stats.Models.BindingModels;

namespace T2Stats.Controllers
{
    [Route("Kills")]
    public class KillsController: Controller
    {
        private const int KillMatchTimeToleranceSeconds = 2;
        private const int MatchStartTimeToleranceSeconds = 10;
        private readonly ApplicationDbContext db;

        public KillsController(ApplicationDbContext db) : base()
        {
            this.db = db;
        }

        [HttpPost]
        [Route("")]
        public IActionResult PostKill()
        {
            KillBindingModel submittedKill = null;
            var reader = new StreamReader(Request.Body);
            var bodyStr = reader.ReadToEnd();
            var newKillEvent = new KillEvent()
            {
                // Event
                EventId = Guid.NewGuid(),
                ReporterName = submittedKill.Reporter.Name,
                ReporterTribesGuid = submittedKill.Reporter.TribesGuid,
                MatchTime = TimeSpan.FromMilliseconds(submittedKill.MatchTimeMs),
                MatchStartTime = submittedKill.Match.StartTime,
                MatchDuration = TimeSpan.FromMinutes(submittedKill.Match.TimeLimitMinutes),
                MatchGameType = submittedKill.Match.GameType,
                MatchMapName = submittedKill.Match.Map.Name,
                ServerName = submittedKill.Match.Server.Name,
                ServerIpAddress = submittedKill.Match.Server.IpAddress,
                ServerPort = submittedKill.Match.Server.Port,
                // Kill Event
                KillerTribesGuid = submittedKill.Killer.TribesGuid,
                KillerName = submittedKill.Killer.Name,
                VictimTribesGuid = submittedKill.Victim.TribesGuid,
                VictimName = submittedKill.Victim.Name,
                KillType = submittedKill.Type,
                Weapon = submittedKill.WeaponName
            };
            db.KillEvents.Add(newKillEvent);
            db.SaveChanges();
            return Ok();
        }
    }
}