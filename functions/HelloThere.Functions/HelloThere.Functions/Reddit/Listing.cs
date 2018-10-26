using Newtonsoft.Json;
using System.Collections.Generic;

namespace HelloThere.Functions.Reddit
{
    public class Listing<T>
    {
        [JsonProperty("children")]
        public List<T> Children { get; private set; }
    }
}
