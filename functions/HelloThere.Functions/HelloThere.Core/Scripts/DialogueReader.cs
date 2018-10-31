using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HelloThere.Core.Scripts
{
    public class DialogueReader : IDialogueReader
    {
        private const string _separator = @": ";
        private readonly Regex _dialogueLineRegex = new Regex(_separator);
        private readonly StreamReader _streamReader;

        public DialogueReader(StreamReader streamReader)
        {
            _streamReader = streamReader;
        }

        public async Task<DialogueLine> ReadDialogueLineAsync()
        {
            int lineNumber = 0;
            string line;
            while ((line = await _streamReader.ReadLineAsync()) != null)
            {
                lineNumber++;

                // ignore lines that only contain whitespace
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // ignore non-dialogue lines
                if (!_dialogueLineRegex.IsMatch(line))
                {
                    continue;
                }

                // only split on the first match
                var splitLine = _dialogueLineRegex.Split(line, 1);
                
                var character = splitLine[0];
                var text = splitLine[1];

                var dialogueLine = new DialogueLine
                {
                    Character = character,
                    Line = lineNumber,
                    Text = text
                };

                return dialogueLine;
            }

            return null;
        }
        
        public void Dispose()
        {
            _streamReader.Dispose();
        }
    }
}
