using System.ComponentModel.DataAnnotations;

namespace TalkToMe.Core.DTO.Request
{
    public class FeedbackRequestDto
    {
        public string? Id { get; set; } = Guid.NewGuid().ToString();

        public string? UserId { get; set; } = default!;

        [Required]
        public string Content { get; set; } = default!;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
