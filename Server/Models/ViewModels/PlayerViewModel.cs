using System;
using Newtonsoft.Json;

namespace T2Stats.Models.ViewModels
{
    public class PlayerViewModel
    {
        [JsonProperty("playerId")]
        public Guid PlayerId { get; set; }

        [JsonProperty("tribesGuid")]
        public string TribesGuid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}