using System.ComponentModel.DataAnnotations;

namespace TalkToMe.Core.DTO.Request;

public class AddWordToDictionaryRequestDto
{
    [Required]
    public string Word { get; set; }
    
    [Required]
    public string Translation { get; set; }
    
    [Required]
    public string Example { get; set; }
    
    [Required]
    public bool IncludeIntoChat { get; set; }
}