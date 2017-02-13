using System;
using Newtonsoft.Json;

namespace T2Stats.Models.BindingModels
{
    public class KillBindingModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("weaponName")]
        public string WeaponName { get; set; }

        // TODO: Remove with auth.
        [JsonProperty("reporter")]
        public PlayerBindingModel Reporter { get; set; }

        [JsonProperty("victim")]
        public PlayerBindingModel Victim { get; set; }

        [JsonProperty("killer")]
        public PlayerBindingModel Killer { get; set; }
        
        [JsonProperty("matchTimeMs")]
        public long MatchTimeMs { get; set; }

        [JsonProperty("match")]
        public MatchBindingModel Match { get; set; }
    }
}