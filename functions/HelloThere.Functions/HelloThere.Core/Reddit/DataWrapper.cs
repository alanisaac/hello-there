using Newtonsoft.Json;

namespace HelloThere.Core.Reddit
{
    public class DataWrapper<T>
    {
        [JsonProperty("data")]
        public T Data { get; private set; }
    }
}
