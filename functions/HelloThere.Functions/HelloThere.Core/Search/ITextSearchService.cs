using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HelloThere.Core.Search
{
    public interface ITextSearchService
    {
        Task<IEnumerable<TextSearchMatch>> FindMatchesAsync(Stream inputTextStream, ICollection<string> searchTerms);
    }
}
