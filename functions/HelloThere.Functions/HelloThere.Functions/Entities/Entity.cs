using Newtonsoft.Json;

namespace HelloThere.Functions.Entities
{
    public class Entity
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
