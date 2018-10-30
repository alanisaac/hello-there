using Flurl;
using Flurl.Http;
using HelloThere.Core.Utilities;
using System;
using System.Threading.Tasks;

namespace HelloThere.Core.Pushshift
{
    public class PushshiftClient : IPushshiftClient
    {
        private const string _pushshift = "https://api.pushshift.io/";

        public async Task<SearchSubmissionsResult> SearchSubmissionsAsync(string subredditName, DateTime? after)
        {
            var url = _pushshift
                .AppendPathSegments("reddit", "search", "submission")
                .SetQueryParams(new
                {
                    subreddit = subredditName,
                    sort = "asc",
                    over_18 = false,
                    is_video = false,
                    after = after != null ? (long?)after.Value.ToUnixTime() : null
                });

            return await url.GetJsonAsync<SearchSubmissionsResult>();
        }
    }
}
