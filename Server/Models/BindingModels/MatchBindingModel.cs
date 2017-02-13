using System;
using Newtonsoft.Json;

namespace T2Stats.Models.BindingModels
{
    public class MatchBindingModel
    {
        [JsonProperty("startTime")]
        public DateTimeOffset StartTime { get; set; }

        [JsonProperty("timeLimitMinutes")]
        public double TimeLimitMinutes { get; set; }

        [JsonProperty("map")]
        public MapBindingModel Map { get; set; }

        [JsonProperty("gameType")]
        public string GameType { get; set; }

        [JsonProperty("server")]
        public ServerBindingModel Server { get; set; }
    }
}