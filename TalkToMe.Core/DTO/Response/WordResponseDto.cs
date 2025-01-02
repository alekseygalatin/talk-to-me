namespace TalkToMe.Core.DTO.Response;

public class WordResponseDto
{
    public string Language { get; set; } = default!;
    public string Word { get; set; } = default!;
    public string Transcription { get; set; } = default!;
    public string BaseFormWord { get; set; } = default!;
    public List<string> Translations { get; set; } = new List<string>();
    public string Example { get; set; } = default!;
    public bool IncludeIntoChat { get; set; }
}