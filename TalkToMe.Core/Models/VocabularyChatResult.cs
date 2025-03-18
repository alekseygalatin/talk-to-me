using TalkToMe.Core.Enums;

namespace TalkToMe.Core.Models
{
    public class VocabularyChatResult
    {
        public string Response { get; set; } = default!;
        public bool Success { get; set; }
        public VocabularyChatSessionStatus Status { get; set; }
    }
}
