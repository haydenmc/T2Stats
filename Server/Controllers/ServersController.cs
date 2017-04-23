using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T2Stats.Models;
using T2Stats.Models.ViewModels;

namespace T2Stats.Controllers
{
    [Route("api/Servers")]
    public class ServersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;

        public ServersController(ApplicationDbContext db, IMapper mapper) : base()
        {
            this.db = db;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetServers()
        {
            var servers = db.Servers.ToList();
            var serversViewModel
                = mapper.Map<ICollection<ServerViewModel>>(servers);
            return Ok(serversViewModel);
        }

        [HttpGet]
        [Route("{serverIpAddress}/{serverPort}")]
        public IActionResult GetMatches(string serverIpAddress, string serverPort)
        {
            var matches = db.Matches.OrderByDescending(m => m.StartTime).ToList();
            var matchesViewModel = mapper.Map<ICollection<MatchViewModel>>(matches);
            return Ok(matchesViewModel);
        }
    }
}