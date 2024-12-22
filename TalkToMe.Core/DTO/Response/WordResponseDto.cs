namespace TalkToMe.Core.DTO.Response;

public class WordResponseDto
{
    public string Word { get; set; }
    public string Translation { get; set; }
    public string Example { get; set; }
    public bool IncludeIntoChat { get; set; }
}