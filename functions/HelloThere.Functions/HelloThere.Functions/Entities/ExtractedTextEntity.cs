using Newtonsoft.Json;
using System.Collections.Generic;

namespace HelloThere.Functions.Entities
{
    public class ExtractedTextEntity : Entity
    {
        [JsonProperty("textLines")]
        public IList<string> TextLines { get; set; }

        [JsonProperty("post")]
        public RedditPostEntity Post { get; set; }
    }
}
