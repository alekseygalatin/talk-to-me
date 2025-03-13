using TalkToMe.Core.Enums;

namespace TalkToMe.Core.Models
{
    public class VocabularyChatSession
    {
        public VocabularyChatSession() : this(new List<string>()) { }

        public VocabularyChatSession(List<string> words)
        {
            Words = words ?? new List<string>();
        }

        private int currentWordIndex { get; set; }
        public List<string> Words { get; set; } = new List<string>();
        
        public VocabularyChatSessionStatus Status { get; set; }

        public string CurrentWord => (currentWordIndex < Words.Count) ? Words[currentWordIndex] : string.Empty;

        public void MoveToNextWord() => currentWordIndex = (currentWordIndex + 1) % Words.Count;
    }
}
