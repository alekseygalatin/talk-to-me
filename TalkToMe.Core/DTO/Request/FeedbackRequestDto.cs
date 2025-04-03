using System.ComponentModel.DataAnnotations;

namespace TalkToMe.Core.DTO.Request
{
    public class FeedbackRequestDto
    {
        public string? UserId { get; set; } = default!;

        [Required]
        public string Content { get; set; } = default!;

        public long? CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
