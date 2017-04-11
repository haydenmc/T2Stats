using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using T2Stats.Models;
using T2Stats.Models.BindingModels;
using T2Stats.Services;

namespace T2Stats.Controllers
{
    [Route("Kills")]
    public class KillsController: Controller
    {
        private const int KillMatchTimeToleranceSeconds = 2;
        private const int MatchStartTimeToleranceSeconds = 10;
        private readonly EventIngestionService eventIngestionService;

        public KillsController(EventIngestionService eventIngestionService) : base()
        {
            this.eventIngestionService = eventIngestionService;
        }

        [HttpPost]
        [Route("")]
        public IActionResult PostKill([FromBody] KillBindingModel submittedKill)
        {
            var newKillEvent = new KillEvent()
            {
                // Event
                EventId = Guid.NewGuid(),
                EventReports = new List<EventReporter>(),
                MatchTime = TimeSpan.FromMilliseconds(submittedKill.MatchTimeMs),
                // Kill Event
                KillerTribesGuid = submittedKill.Killer.TribesGuid,
                KillerName = submittedKill.Killer.Name,
                VictimTribesGuid = submittedKill.Victim.TribesGuid,
                VictimName = submittedKill.Victim.Name,
                KillType = submittedKill.Type,
                Weapon = submittedKill.WeaponName
            };
            eventIngestionService.RecordEvent(newKillEvent, submittedKill.Reporter, submittedKill.Match);
            return Ok();
        }
    }
}