using System.IO;
using System.Threading.Tasks;

namespace HelloThere.Core.Scripts
{
    /// <summary>
    /// A script repository on the local file system
    /// </summary>
    public class LocalScriptRepository : IScriptRepository
    {
        private string _directory;

        public LocalScriptRepository(string directory)
        {
            _directory = directory;
        }

        public Task<Stream> GetScriptStreamAsync(string name)
        {
            var path = Path.Combine(_directory, name);
            var stream = File.OpenRead(path);
            return Task.FromResult<Stream>(stream);
        }
    }
}
