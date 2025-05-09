using AutoMapper;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.Exceptions;
using TalkToMe.Core.Interfaces;
using TalkToMe.Domain.Entities;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Core.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IBaseRepository<Subscription> _repository;
        private readonly IMapper _mapper;

        public SubscriptionService(IBaseRepository<Subscription> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task RequestSubscription(SubscriptionRequestDto subscription)
        {
            if (string.IsNullOrWhiteSpace(subscription.UserId))
                throw new Exception("User id cannot be empty");

            if (await _repository.GetByIdAsync(subscription.UserId) != null)
                throw new UserFriendlyException("Subsciption already requested");

            var maxLength = 500;
            if (subscription.Comment.Length > maxLength)
                throw new UserFriendlyException($"Subscription comment maximum length is {maxLength}");

            await _repository.CreateAsync(_mapper.Map<Subscription>(subscription));
        }

        public async Task<bool> SubscriptionRequested(string userId) 
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new Exception("User id cannot be empty");

            return await _repository.GetByIdAsync(userId) != null;
        }

        public async Task CancelSubscriptionRequest(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new Exception("User id cannot be empty");

            await _repository.DeleteAsync(userId);
        }
    }
}
