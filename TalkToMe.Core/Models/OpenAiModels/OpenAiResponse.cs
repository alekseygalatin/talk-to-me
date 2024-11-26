namespace TalkToMe.Core.Models.OpenAiModels
{
    public class OpenAiResponse
    {
        public List<OpenAiChoice> Choices { get; set; } = default!;
    }
}
