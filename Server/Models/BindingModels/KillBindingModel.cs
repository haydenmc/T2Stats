using System;
using Newtonsoft.Json;

namespace T2Stats.Models.BindingModels
{
    public class KillBindingModel
    {
        [JsonProperty("weaponName")]
        public string WeaponName { get; set; }

        [JsonProperty("victim")]
        public PlayerBindingModel Victim { get; set; }

        [JsonProperty("killer")]
        public PlayerBindingModel Killer { get; set; }
        
        [JsonProperty("matchTime")]
        public TimeSpan MatchTime { get; set; }

        [JsonProperty("match")]
        public MatchBindingModel Match { get; set; }
    }
}