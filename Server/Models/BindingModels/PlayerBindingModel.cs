using Newtonsoft.Json;

namespace T2Stats.Models.BindingModels
{
    public class PlayerBindingModel
    {
        [JsonProperty("tribesGuid")]
        public string TribesGuid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}