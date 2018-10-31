using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloThere.Core.Scripts
{
    public class DialogueLineIndex : IDialogueLineIndex
    {
        private readonly IElasticClient _elasticClient;
        private readonly string _indexName = "dialogue-lines";

        public DialogueLineIndex(Uri hostUri)
        {
            var settings = new ConnectionSettings(hostUri);
            settings.DefaultMappingFor<ScriptDialogueLine>(x => x.IndexName(_indexName));
            _elasticClient = new ElasticClient(settings);
        }

        public async Task BulkIndexDialogueLinesAsync(IEnumerable<ScriptDialogueLine> dialogueLines)
        {
            var dialogueLinesToIndex = dialogueLines.ToList();
            var taskCompletionSource = new TaskCompletionSource<bool>();

            var bulkAll = _elasticClient.BulkAll(dialogueLinesToIndex, b => b
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

        public async Task<ScriptDialogueLine> SearchAsync(string phrase)
        {
            var searchResults = await _elasticClient.SearchAsync<ScriptDialogueLine>(x => x
                .Query(q => q
                    .MatchPhrase(c => c
                        .Field(p => p.Text)
                        .Query(phrase))));

            var topResult = searchResults.Documents.FirstOrDefault();
            return topResult;
        }
    }
}
