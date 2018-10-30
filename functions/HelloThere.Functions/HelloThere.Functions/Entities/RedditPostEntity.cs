using Newtonsoft.Json;
using System;

namespace HelloThere.Functions.Entities
{
    public class RedditPostEntity : Entity
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("created_utc")]
        public DateTime CreatedUtc { get; set; }
    }
}
