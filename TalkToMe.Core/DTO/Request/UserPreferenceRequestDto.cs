using System.ComponentModel.DataAnnotations;

namespace TalkToMe.Core.DTO.Request
{
    public class UserPreferenceRequestDto
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string NativeLanguage { get; set; } = default!;

        [Required]
        public string PreferedPronoun { get; set; } = default!;

        public string? CurrentLanguageToLearn { get; set; } = default!;
    }
}
