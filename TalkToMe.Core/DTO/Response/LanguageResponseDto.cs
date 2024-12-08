using System.ComponentModel.DataAnnotations;

namespace TalkToMe.Core.DTO.Response
{
    public class LanguageResponseDto
    {
        [Required]
        public string Code { get; set; } = default!;

        [Required]
        public string Name { get; set; } = default!;
    }
}
