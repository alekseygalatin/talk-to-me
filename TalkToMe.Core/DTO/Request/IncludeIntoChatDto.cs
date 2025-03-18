namespace TalkToMe.Core.DTO.Request
{
    public class IncludeIntoChatDto
    {
        public string LanguageCode { get; set; } = default!;
        public string Word { get; set; } = default!;
        public bool IncludeIntoChat { get; set; } = default!;
    }
}
