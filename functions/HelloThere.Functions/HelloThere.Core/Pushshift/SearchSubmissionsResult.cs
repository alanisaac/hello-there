using Newtonsoft.Json;
using System.Collections.Generic;

namespace HelloThere.Core.Pushshift
{
    public class SearchSubmissionsResult
    {
        [JsonProperty("data")]
        public IList<Submission> Data { get; private set; }
    }
}
