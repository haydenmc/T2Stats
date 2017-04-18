using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace T2Stats.Models.ViewModels
{
    public class KillEventViewModel
    {
        [JsonProperty("eventId")]
        public Guid EventId { get; set; }

        [JsonProperty("matchId")]
        public Guid MatchId { get; set; }

        [JsonProperty("matchTime")]
        public TimeSpan MatchTime { get; set; }

        [JsonProperty("killer")]
        public PlayerViewModel Killer { get; set; }

        [JsonProperty("victim")]
        public PlayerViewModel Victim { get; set; }

        [JsonProperty("killType")]
        public string KillType { get; set; }

        [JsonProperty("weapon")]
        public string Weapon { get; set; }

        [JsonProperty("reporters")]
        public ICollection<PlayerViewModel> Reporters { get; set; }
    }
}