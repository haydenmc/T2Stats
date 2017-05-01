using System;
using Newtonsoft.Json;

namespace T2Stats.Models.ViewModels
{
    public class MatchViewModel
    {
        [JsonProperty("matchId")]
        public Guid MatchId { get; set; }

        [JsonProperty("server")]
        public ServerViewModel Server { get; set; }

        [JsonProperty("mapName")]
        public string MapName { get; set; }

        [JsonProperty("gameType")]
        public string GameType { get; set; }

        [JsonProperty("startTime")]
        public DateTimeOffset StartTime { get; set; }

        [JsonProperty("duration")]
        public TimeSpan Duration { get; set; }
    }
}