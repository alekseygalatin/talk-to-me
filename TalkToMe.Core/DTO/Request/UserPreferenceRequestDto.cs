using System.ComponentModel.DataAnnotations;

namespace TalkToMe.Core.DTO.Request
{
    public class UserPreferenceRequestDto
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Sex { get; set; } = default!;

        [Required]
        public string NativeLanguage { get; set; } = default!;
    }
}
