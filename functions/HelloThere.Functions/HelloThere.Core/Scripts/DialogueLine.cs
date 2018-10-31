namespace HelloThere.Core.Scripts
{
    public class DialogueLine
    {
        /// <summary>
        /// The character who speaks the line of dialogue.
        /// </summary>
        public string Character { get; set; }

        /// <summary>
        /// The line number of the start of the dialogue within the original script.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// The full text of the dialogue, without line breaks.
        /// </summary>
        public string Text { get; set; }
    }
}
