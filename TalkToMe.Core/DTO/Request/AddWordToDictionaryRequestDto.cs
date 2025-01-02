using System.ComponentModel.DataAnnotations;

namespace TalkToMe.Core.DTO.Request;

public class AddWordToDictionaryRequestDto
{
    [Required]
    public string Language { get; set; } = default!;

    [Required]
    public string Word { get; set; } = default!;

    [Required]
    public string Transcription { get; set; } = default!;

    [Required]
    public string BaseFormWord { get; set; } = default!;

    [Required]
    public List<string> Translations { get; set; } = new List<string>();

    [Required]
    public string Example { get; set; } = default!;
}