using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelloThere.Core.OCR;
using HelloThere.Core.Pushshift;
using HelloThere.Core.Utilities;
using HelloThere.Functions.Entities;
using HelloThere.Functions.OCR;
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
            [TimerTrigger("0 */5 * * * *", RunOnStartup = false)]TimerInfo myTimer,
            [CosmosDB("prequelmemes", "redditPosts", CreateIfNotExists = true,
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<RedditPostEntity> redditPosts,
            ILogger logger)
        {
            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            IPushshiftClient pushshiftClient = new PushshiftClient();
            var searchSubmissionsResult = await pushshiftClient.SearchSubmissionsAsync("prequelmemes", null);
            
            var postsToProcess = searchSubmissionsResult.Data;

            foreach (var post in postsToProcess)
            {
                var redditPost = new RedditPostEntity
                {
                    Id = post.Id,
                    Title = post.Title,
                    Url = post.Url.ToString(),
                    Permalink = post.Permalink.ToString(),
                    Score = post.Score,
                    CreatedUtc = post.CreatedUtc.FromUnixTime()
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
            
            var endpoint = "https://westcentralus.api.cognitive.microsoft.com";
            var subscriptionKey = config["ComputerVisionSubscriptionKey"];
            IOCRClient ocrClient = new OCRClient(endpoint, subscriptionKey, logger);

            foreach (var document in documents)
            {
                var redditPost = JsonConvert.DeserializeObject<RedditPostEntity>(document.ToString());
                
                logger.LogInformation($"Attempting to extract text from '{redditPost.Permalink}'");
                var result = await ocrClient.ExtractTextAsync(redditPost.Url);

                // free subscription allows 20 calls per minute
                await Task.Delay(TimeSpan.FromSeconds(5));

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
