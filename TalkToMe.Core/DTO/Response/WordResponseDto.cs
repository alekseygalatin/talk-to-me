using System.ComponentModel.DataAnnotations;

namespace TalkToMe.Core.DTO.Response;

public class WordResponseDto
{
    public string LanguageWord { get { return $"{Language}#{Word}"; } }
    public string Transcription { get; set; } = default!;
    public string Language { get; set; } = default!;
    public string Word { get; set; } = default!;
    public List<string> Translations { get; set; } = new List<string>();
    public string Example { get; set; } = default!;
    public bool IncludeIntoChat { get; set; }
}