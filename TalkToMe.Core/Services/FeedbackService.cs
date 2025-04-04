using AutoMapper;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.Exceptions;
using TalkToMe.Core.Interfaces;
using TalkToMe.Domain.Entities;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Core.Services
{
    public class FeedbackService: IFeedbackService
    {
        private readonly IFeedbackRepository _repository;
        private readonly IMapper _mapper;

        public FeedbackService(IFeedbackRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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

            var lastFeedbackDate = await _repository.GetLastFeedbackDate(feedback.UserId);
            if (lastFeedbackDate != null) 
            {
                if ((DateTime.UtcNow - lastFeedbackDate.Value).TotalMinutes < 60)
                    throw new UserFriendlyException("You can submit feedback only once per hour");
            }

            await _repository.CreateAsync(_mapper.Map<Feedback>(feedback));
        }

    }
}
