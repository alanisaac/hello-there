using HelloThere.Core.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HelloThere.CLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await MatchPhraseTest();
        }

        private static async Task MatchPhraseTest()
        {
            IDialogueLineIndex index = new DialogueLineIndex(new Uri("http://localhost:9200"));

            var searchPhrase = "hello there";

            Console.WriteLine("Searching for the phrase:");
            Console.WriteLine(searchPhrase);
            var result = await index.SearchAsync(searchPhrase);
            Console.WriteLine("Search completed.");

            if (result != null)
            {
                Console.WriteLine($"On line {result.Line} of {result.Source}, {result.Character} says '{result.Text}'");
            }
            else
            {
                Console.WriteLine($"Could not find that phrase in the scripts.");
            }
            Console.ReadKey();
        }

        private static async Task ImportScriptsTest()
        {
            IScriptRepository scriptRepository =
                new LocalScriptRepository(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            IDialogueLineIndex index = new DialogueLineIndex(new Uri("http://localhost:9200"));

            var scriptNames = new List<string>
            {
                "episodeI.txt",
                //"episodeII.txt",
                "episodeIII.txt",
            };

            foreach (var scriptName in scriptNames)
            {
                using (var scriptStream = await scriptRepository.GetScriptStreamAsync(scriptName))
                {
                    var streamReader = new StreamReader(scriptStream);
                    var dialogueReader = new DialogueReader(streamReader);

                    DialogueLine line;
                    var buffer = new List<ScriptDialogueLine>();
                    while ((line = await dialogueReader.ReadDialogueLineAsync()) != null)
                    {
                        var scriptDialogueLine = ConvertToScriptDialogueLine(scriptName, line);
                        if (buffer.Count == 25)
                        {
                            await index.BulkIndexDialogueLinesAsync(buffer);
                            buffer.Clear();
                        }
                        buffer.Add(scriptDialogueLine);
                    }
                }
            }
        }

        private static ScriptDialogueLine ConvertToScriptDialogueLine(string scriptName, DialogueLine dialogueLine)
        {
            return new ScriptDialogueLine
            {
                Source = scriptName,
                Character = dialogueLine.Character,
                Line = dialogueLine.Line,
                Text = dialogueLine.Text,
            };
        }
    }
}
