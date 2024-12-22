using System.ComponentModel.DataAnnotations;

namespace TalkToMe.Core.DTO.Response
{
    public class LanguageResponseDto
    {
        [Required]
        public string Code { get; set; } = default!;

        [Required]
        public string Name { get; set; } = default!;

        public string EnglishName { get; set; } = default!;
        public List<string> Pronouns { get; set; } = new List<string>();
    }
}
