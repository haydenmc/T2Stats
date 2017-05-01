using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using T2Stats.Models;
using T2Stats.Models.ViewModels;

namespace T2Stats.Controllers
{
    [Route("api/Players")]
    public class PlayersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;

        public PlayersController(ApplicationDbContext db, IMapper mapper) : base()
        {
            this.db = db;
            this.mapper = mapper;
        }
        
        [HttpGet]
        [Route("")]
        public IActionResult GetRecentPlayers()
        {
            var recentPlayers = db.Events
                .OrderByDescending(e => e.Match.StartTime.Add(e.MatchTime))
                .SelectMany(e => e.EventReports.Select(er => er.Player))
                .Distinct()
                .Take(50);
            return Ok(mapper.Map<ICollection<PlayerViewModel>>(recentPlayers));
        }
    }
}