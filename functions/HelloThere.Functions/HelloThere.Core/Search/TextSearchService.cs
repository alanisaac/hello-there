using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HelloThere.Core.Search
{
    public class TextSearchService : ITextSearchService
    {
        public async Task<IEnumerable<TextSearchMatch>> FindMatchesAsync(Stream inputTextStream, ICollection<string> searchTerms)
        {
            var matches = new List<TextSearchMatch>();

            var streamReader = new StreamReader(inputTextStream);
            string line;
            int lineNumber = 0;

            while ((line = await streamReader.ReadLineAsync()) != null)
            {
                lineNumber++;

                // ignore lines that only contain whitespace
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // a naive implementation of string matching, only accepting exact matches
                // TODO: this will not work in most cases, and will need to be enhanced with fuzzy matching
                foreach (var searchTerm in searchTerms)
                {
                    int characterNumber = -1;
                    if ((characterNumber = line.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase)) != -1)
                    {
                        var match = new TextSearchMatch
                        {
                            SearchText = searchTerm,
                            LineNumber = lineNumber,
                            CharacterNumber = characterNumber
                        };
                        matches.Add(match);
                    }
                }
            }

            return matches;
        }
    }
}
