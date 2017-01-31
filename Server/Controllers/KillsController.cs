using System;
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
        public IActionResult PostKill([FromBody] KillBindingModel submittedKill)
        {
            if (submittedKill.Match?.Server != null)
            {
                // First, verify Server
                var dbServer = db.Servers.FirstOrDefault(s => 
                    s.IpAddress == submittedKill.Match.Server.IpAddress &&
                    s.Port == submittedKill.Match.Server.Port);
                if (dbServer == null)
                {
                    dbServer = new Models.Server()
                    {
                        ServerId = Guid.NewGuid(),
                        IpAddress = submittedKill.Match.Server.IpAddress,
                        Port = submittedKill.Match.Server.Port
                    };
                    db.Servers.Add(dbServer);
                    db.SaveChanges();
                }

                // Then, verify Match
                var dbMatch = db.Matches.Include(m => m.Map).FirstOrDefault(m =>
                    m.ServerId == dbServer.ServerId && 
                    m.StartTime > submittedKill.Match.StartTime.AddSeconds(-MatchStartTimeToleranceSeconds) &&
                    m.StartTime < DateTimeOffset.Now && 
                    m.Map.Name.ToLower() == submittedKill.Match.Map.Name.ToLower());
                if (dbMatch == null)
                {
                    var dbMap = db.Maps.FirstOrDefault(m => 
                        m.Name == submittedKill.Match.Map.Name);
                    if (dbMap == null)
                    {
                        dbMap = new Map()
                        {
                            MapId = Guid.NewGuid(),
                            Name = submittedKill.Match.Map.Name
                        };
                        db.Maps.Add(dbMap);
                        db.SaveChanges();
                    }
                    dbMatch = new Match()
                    {
                        MatchId = Guid.NewGuid(),
                        StartTime = submittedKill.Match.StartTime,
                        Duration = submittedKill.Match.Duration,
                        MapId = dbMap.MapId,
                        GameType = submittedKill.Match.GameType,
                        ServerId = dbServer.ServerId
                    };
                    db.Matches.Add(dbMatch);
                    db.SaveChanges();
                }

                // Populate reporter
                var dbReporter = db.Players.FirstOrDefault(p =>
                    p.TribesGuid == submittedKill.Reporter.TribesGuid);
                if (dbReporter == null)
                {
                    dbReporter = new Player()
                    {
                        PlayerId = Guid.NewGuid(),
                        TribesGuid = submittedKill.Reporter.TribesGuid,
                        Name = submittedKill.Reporter.Name
                    };
                    db.Players.Add(dbReporter);
                    db.SaveChanges();
                }

                // Populate victim
                var dbVictim = db.Players.FirstOrDefault(p =>
                    p.TribesGuid == submittedKill.Victim.TribesGuid);
                if (dbVictim == null)
                {
                    dbVictim = new Player()
                    {
                        PlayerId = Guid.NewGuid(),
                        TribesGuid = submittedKill.Victim.TribesGuid,
                        Name = submittedKill.Victim.Name
                    };
                    db.Players.Add(dbVictim);
                    db.SaveChanges();
                }

                // Populate killer
                var dbKiller = db.Players.FirstOrDefault(p =>
                    p.TribesGuid == submittedKill.Killer.TribesGuid);
                if (dbKiller == null)
                {
                    dbKiller = new Player()
                    {
                        PlayerId = Guid.NewGuid(),
                        TribesGuid = submittedKill.Killer.TribesGuid,
                        Name = submittedKill.Killer.Name
                    };
                    db.Players.Add(dbKiller);
                    db.SaveChanges();
                }

                // Populate weapon
                var dbWeapon = db.Weapons.FirstOrDefault(w =>
                    w.Name.ToLower() == submittedKill.WeaponName.ToLower());
                if (dbWeapon == null)
                {
                    dbWeapon = new Weapon()
                    {
                        WeaponId = Guid.NewGuid(),
                        Name = submittedKill.WeaponName
                    };
                    db.Weapons.Add(dbWeapon);
                    db.SaveChanges();
                }

                // Then, verify Kill
                var dbKill = db.Kills.FirstOrDefault(k => 
                    k.Killer.TribesGuid == submittedKill.Killer.TribesGuid &&
                    k.Victim.TribesGuid == submittedKill.Victim.TribesGuid && 
                    k.Weapon.Name.ToLower() == submittedKill.WeaponName.ToLower() &&
                    k.MatchTime > submittedKill.MatchTime.Subtract(TimeSpan.FromSeconds(KillMatchTimeToleranceSeconds)) &&
                    k.MatchTime < submittedKill.MatchTime.Add(TimeSpan.FromSeconds(KillMatchTimeToleranceSeconds)) &&
                    k.MatchId == dbMatch.MatchId);
                if (dbKill == null)
                {
                    dbKill = new Kill()
                    {
                        KillId = Guid.NewGuid(),
                        KillerId = dbKiller.PlayerId,
                        VictimId = dbVictim.PlayerId,
                        ReporterId = dbReporter.PlayerId,
                        WeaponId = dbWeapon.WeaponId,
                        MatchTime = submittedKill.MatchTime,
                        MatchId = dbMatch.MatchId
                    };
                    db.Kills.Add(dbKill);
                    db.SaveChanges();
                }
                return Ok();
            }
            else
            {
                return BadRequest("Match and server properties are required.");
            }
        }
    }
}