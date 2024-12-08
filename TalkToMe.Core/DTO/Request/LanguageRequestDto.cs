using System.ComponentModel.DataAnnotations;

namespace TalkToMe.Core.DTO.Request
{
    public class LanguageRequestDto
    {
        [Required]
        public string Code { get; set; } = default!;

        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string EnglishName { get; set; } = default!;

        public bool Active { get; set; }
    }
}
