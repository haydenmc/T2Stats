using System;
using Newtonsoft.Json;

namespace T2Stats.Models.ViewModels
{
    public class ServerViewModel
    {
        [JsonProperty("serverId")]
        public Guid ServerId { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("port")]
        public string Port { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}