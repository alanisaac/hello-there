using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelloThere.Functions.OCR
{
    public interface IOCRClient
    {
        /// <summary>
        /// Extract lines of text from an image at the given URL.
        /// </summary>
        Task<IList<string>> ExtractTextAsync(string imageUrl);
    }
}
