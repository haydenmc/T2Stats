using Newtonsoft.Json;

namespace T2Stats.Models.BindingModels
{
    public class MapBindingModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}