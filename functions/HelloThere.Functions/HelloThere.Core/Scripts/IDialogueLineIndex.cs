using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelloThere.Core.Scripts
{
    public interface IDialogueLineIndex
    {
        Task BulkIndexDialogueLinesAsync(IEnumerable<ScriptDialogueLine> dialogueLines);
    }
}
