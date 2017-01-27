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

        [JsonProperty("reporter")]
        public PlayerBindingModel Reporter { get; set; }
    }
}