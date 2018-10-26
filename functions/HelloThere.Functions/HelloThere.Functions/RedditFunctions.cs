using System;
using System.Threading.Tasks;
using HelloThere.Functions.Reddit;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace HelloThere.Functions
{
    public static class RedditFunctions
    {
        [FunctionName(nameof(FetchPosts))]
        public static async Task FetchPosts([TimerTrigger("0 */5 * * * *", RunOnStartup = true)]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            IRedditClient redditClient = new RedditClient();
            var posts = await redditClient.GetTopPostsAsync("prequelmemes", 20);
        }
    }
}
