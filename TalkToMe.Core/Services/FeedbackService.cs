using TalkToMe.Core.DTO.Extensions;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.Exceptions;
using TalkToMe.Core.Interfaces;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Core.Services
{
    public class FeedbackService: IFeedbackService
    {
        private readonly IFeedbackRepository _repository;

        public FeedbackService(IFeedbackRepository repository)
        {
            _repository = repository;
        }

        public async Task SaveFeedbackAsync(FeedbackRequestDto feedback)
        {
            if (string.IsNullOrWhiteSpace(feedback.UserId))
                throw new Exception("User id cannot be empty");

            if (string.IsNullOrWhiteSpace(feedback.Content))
                throw new UserFriendlyException("Feedback content cannot be empty");

            var maxLength = 500;
            if (feedback.Content.Length > maxLength)
                throw new UserFriendlyException($"Feedback content maximum length is {maxLength}");

            var lastFeedback = await _repository.GetLastFeedbackDate(feedback.UserId);

            if (lastFeedback.HasValue)
            {
                var elapsed = DateTimeOffset.UtcNow - DateTimeOffset.FromUnixTimeSeconds(lastFeedback.Value);

                if (elapsed.TotalSeconds < 3600)
                {
                    var minutesRemaining = Math.Ceiling((3600 - elapsed.TotalSeconds) / 60);
                    throw new UserFriendlyException($"You can submit feedback again in {minutesRemaining} minutes.");
                }
            }

            await _repository.CreateAsync(feedback.ToEntity());
        }

    }
}
