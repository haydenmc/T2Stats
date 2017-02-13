using Newtonsoft.Json;

namespace T2Stats.Models.BindingModels
{
    public class ServerBindingModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }
    }
}