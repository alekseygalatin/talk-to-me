namespace TalkToMe.Core.DTO.Response
{
    public class UserPreferenceResponseDto
    {
        public string UserId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Sex { get; set; } = default!;
        public string NativeLanguage { get; set; } = default!;
    }
}
