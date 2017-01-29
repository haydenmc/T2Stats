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
        [Route("/")]
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
                    dbServer = new Server()
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
                        
                    }
                }
            }
            else
            {
                return BadRequest("Match, server properties are required.");
            }
        }
    }
}