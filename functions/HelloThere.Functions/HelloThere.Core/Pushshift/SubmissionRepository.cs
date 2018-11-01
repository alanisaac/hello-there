using HelloThere.Core.Utilities;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloThere.Core.Pushshift
{
    public class SubmissionRepository : ISubmissionRepository
    {
        private readonly IElasticClient _elasticClient;
        private readonly string _indexName = "submissions";

        public SubmissionRepository(Uri hostUri)
        {
            var settings = new ConnectionSettings(hostUri);
            settings.DefaultMappingFor<Submission>(x => x.IndexName(_indexName));
            _elasticClient = new ElasticClient(settings);
        }

        public async Task BulkInsertSubmissionsAsync(IEnumerable<Submission> submissions)
        {
            var itemsToIndex = submissions.ToList();
            var taskCompletionSource = new TaskCompletionSource<bool>();

            var bulkAll = _elasticClient.BulkAll(itemsToIndex, b => b
                .BackOffRetries(2)
                .BackOffTime("30s")
                .RefreshOnCompleted(true)
                .MaxDegreeOfParallelism(4)
                .Size(1000)
            );

            bulkAll.Subscribe(new BulkAllObserver(
                onNext: (b) => { /* possibly add progress */ },
                onError: (e) => { taskCompletionSource.SetException(e); },
                onCompleted: () => taskCompletionSource.SetResult(true)
            ));

            await taskCompletionSource.Task;
        }

        public async Task<DateTime?> GetMaxCreatedDateAsync()
        {
            var response = await _elasticClient.SearchAsync<Submission>(x => x
                .Aggregations(a => a
                    .Max("max_created_utc", m => m
                        .Field(p => p.CreatedUtc)
                    )));

            if (!response.IsValid)
            {
                return null;
            }

            var maxCreatedUtc = response.Aggregations.Max("max_created_utc").Value;
            if (maxCreatedUtc == null)
            {
                return null;
            }

            return ((long)maxCreatedUtc.Value).FromUnixTime();
        }
    }
}
