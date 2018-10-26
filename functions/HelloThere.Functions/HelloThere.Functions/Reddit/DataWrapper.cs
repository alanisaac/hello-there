using Newtonsoft.Json;

namespace HelloThere.Functions.Reddit
{
    public class DataWrapper<T>
    {
        [JsonProperty("data")]
        public T Data { get; private set; }
    }
}
