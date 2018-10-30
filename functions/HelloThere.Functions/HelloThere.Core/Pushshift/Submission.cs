using Newtonsoft.Json;
using System;

namespace HelloThere.Core.Pushshift
{
    public class Submission
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        /// <summary>
        /// Returns true if this post is marked not safe for work.
        /// </summary>
        [JsonProperty("over_18")]
        public bool NSFW { get; private set; }

        /// <summary>
        /// Post title.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; private set; }

        /// <summary>
        /// Post uri.
        /// </summary>
        [JsonProperty("url")]
        public Uri Url { get; private set; }

        /// <summary>
        /// The permalink for an item
        /// </summary>
        [JsonProperty("permalink")]
        public Uri Permalink { get; private set; }

        /// <summary>
        /// Score of this item.
        /// </summary>
        [JsonProperty("score")]
        public int Score { get; private set; }

        /// <summary>
        /// The epoch-based timestamp this submission was created.
        /// </summary>
        [JsonProperty("created_utc")]
        public long CreatedUtc { get; private set; }
    }
}
