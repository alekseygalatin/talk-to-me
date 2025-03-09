using TalkToMe.Core.Enums;

namespace TalkToMe.Core.Models
{
    public class VocabularyChatResult
    {
        public string response { get; set; } = default!;
        public bool success { get; set; }
        public VocabularyChatSessionStatus status { get; set; }
    }
}
