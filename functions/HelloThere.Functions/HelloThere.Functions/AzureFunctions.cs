using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelloThere.Functions.Entities;
using HelloThere.Functions.OCR;
using HelloThere.Functions.Reddit;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HelloThere.Functions
{
    public static class AzureFunctions
    {
        [FunctionName(nameof(FetchPosts))]
        public static async Task FetchPosts(
            [TimerTrigger("0 */5 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [CosmosDB("prequelmemes", "redditPosts", CreateIfNotExists = true,
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<RedditPostEntity> redditPosts,
            ILogger logger)
        {
            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            IRedditClient redditClient = new RedditClient();
            var postResult = await redditClient.GetTopPostsAsync("prequelmemes", 20);

            // Exclude NSFW posts
            var postsToProcess = postResult.Data.Children
                .Select(x => x.Data)
                .Where(x => !x.NSFW);

            foreach (var post in postsToProcess)
            {
                var redditPost = new RedditPostEntity
                {
                    Id = post.Permalink.ToString().Replace("/", "-"),
                    Title = post.Title,
                    Url = post.Url.ToString(),
                    Permalink = post.Permalink.ToString(),
                    Upvotes = post.Upvotes,
                    Downvotes = post.Downvotes
                };
                await redditPosts.AddAsync(redditPost);
            }
        }

        [FunctionName(nameof(ExtractText))]
        public static async Task ExtractText(
            [CosmosDBTrigger("prequelmemes", "redditPosts", CreateLeaseCollectionIfNotExists = true,
                ConnectionStringSetting = "CosmosDBConnection")] IReadOnlyList<Document> documents,
            [CosmosDB("prequelmemes", "extractedTexts", CreateIfNotExists = true,
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<ExtractedTextEntity> extractedTexts,
            ILogger logger)
        {
            logger.LogInformation($"{nameof(ExtractText)}: received {documents.Count} posts to process.");

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

            foreach (var document in documents)
            {
                var redditPost = JsonConvert.DeserializeObject<RedditPostEntity>(document.ToString());
                var result = await ocrClient.ExtractTextAsync(redditPost.Url);

                var extractedText = new ExtractedTextEntity
                {
                    Id = redditPost.Id,
                    Post = redditPost,
                    TextLines = result
                };

                await extractedTexts.AddAsync(extractedText);
            }
        }
    }
}
