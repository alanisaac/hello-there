using System;
using System.Threading.Tasks;
using HelloThere.Functions.OCR;
using HelloThere.Functions.Reddit;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HelloThere.Functions
{
    public static class AzureFunctions
    {
        [FunctionName(nameof(FetchPosts))]
        public static async Task FetchPosts([TimerTrigger("0 */5 * * * *", RunOnStartup = true)]TimerInfo myTimer, ILogger logger)
        {
            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            IRedditClient redditClient = new RedditClient();
            var posts = await redditClient.GetTopPostsAsync("prequelmemes", 20);
        }

        [FunctionName(nameof(ExtractText))]
        public static async Task ExtractText([TimerTrigger("0 */5 * * * *", RunOnStartup = true)]TimerInfo myTimer, ILogger logger)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // hard-coded test
            var computerVisionClient = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(config["ComputerVisionSubscriptionKey"]),
                new System.Net.Http.DelegatingHandler[] { });

            computerVisionClient.Endpoint = "https://westcentralus.api.cognitive.microsoft.com";

            IOCRClient ocrClient = new OCRClient(computerVisionClient, logger);
            var result = await ocrClient.ExtractTextAsync(@"https://i.redd.it/97j2suhazdu11.jpg");
        }
    }
}
