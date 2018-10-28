using System.IO;
using System.Threading.Tasks;

namespace HelloThere.Core.Scripts
{
    public interface IScriptRepository
    {
        Task<Stream> GetScriptStreamAsync(string name);
    }
}
