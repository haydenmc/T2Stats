using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T2Stats.Models;
using T2Stats.Models.ViewModels;

namespace T2Stats.Controllers
{
    [Route("api/Matches")]
    public class MatchesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;

        public MatchesController(ApplicationDbContext db, IMapper mapper) : base()
        {
            this.db = db;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetRecentMatches()
        {
            var matches = db.Matches.Include(m => m.Server).OrderByDescending(m => m.StartTime).Take(50);
            return Ok(mapper.Map<ICollection<MatchViewModel>>(matches));
        }

        [HttpGet]
        [Route("{matchId:guid}")]
        public IActionResult GetMatch(Guid matchId)
        {
            // TODO: Return a match summary object with all events
            var match = db.Matches.Include(m => m.Server).SingleOrDefault(m => m.MatchId == matchId);
            return Ok(mapper.Map<MatchViewModel>(match));
        }

        [HttpGet]
        [Route("{matchId:guid}/Kills")]
        public IActionResult GetMatchKills(Guid matchId)
        {
            // Find the match
            var matchKillEvents = db.KillEvents
                .Include(k => k.Killer)
                .Include(k => k.Victim)
                .Include(k => k.EventReports).ThenInclude(e => e.Player)
                .Where(k => k.MatchId == matchId)
                .ToList();
            // Map to viewmodel
            var matchKillEventsViewModel
                = mapper.Map<ICollection<KillEventViewModel>>(matchKillEvents);
            return Ok(matchKillEventsViewModel);
        }
    }
}