using Flurl;
using Flurl.Http;
using System.Threading.Tasks;

namespace HelloThere.Functions.Reddit
{
    public class RedditClient : IRedditClient
    {
        private const string _reddit = "https://www.reddit.com/";

        public async Task<GetPostsResult> GetNewPostsAsync(string subredditName, int count)
        {
            var url = _reddit
                .AppendPathSegments("r", subredditName, "new", ".json")
                .SetQueryParams(new
                {
                    count
                });

            return await url.GetJsonAsync<GetPostsResult>();
        }

        public async Task<GetPostsResult> GetTopPostsAsync(string subredditName, int count)
        {
            var url = _reddit
                .AppendPathSegments("r", subredditName, "top", ".json")
                .SetQueryParams(new
                {
                    count
                });

            return await url.GetJsonAsync<GetPostsResult>();
        }
    }
}
