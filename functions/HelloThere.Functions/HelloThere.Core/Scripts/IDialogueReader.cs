using System;
using System.Threading.Tasks;

namespace HelloThere.Core.Scripts
{
    public interface IDialogueReader : IDisposable
    {
        /// <summary>
        /// Reads the next line of dialogue.
        /// </summary>
        /// <returns>The next line of dialogue, or <c>null</c> if no more lines of dialogue exist.</returns>
        Task<DialogueLine> ReadDialogueLineAsync();
    }
}
