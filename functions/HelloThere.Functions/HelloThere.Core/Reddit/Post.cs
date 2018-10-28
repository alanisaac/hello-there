using Newtonsoft.Json;
using System;

namespace HelloThere.Core.Reddit
{
    public class Post
    {
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
        /// Number of upvotes on this item.
        /// </summary>
        [JsonProperty("ups")]
        public int Upvotes { get; private set; }
        
        /// <summary>
        /// Number of upvotes on this item.
        /// </summary>
        [JsonProperty("downs")]
        public int Downvotes { get; private set; }
    }
}
