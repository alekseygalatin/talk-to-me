namespace TalkToMe.Core.DTO.Request
{
    public class SubscriptionRequestDto
    {
        public string? UserId { get; set; } = default!;

        public string? Comment { get; set; } = default!;

        public long? CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
