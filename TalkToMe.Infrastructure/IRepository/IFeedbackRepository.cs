using TalkToMe.Domain.Entities;

namespace TalkToMe.Infrastructure.IRepository
{
    public interface IFeedbackRepository : IBaseRepository<Feedback>
    {
        Task<DateTime?> GetLastFeedbackDate(string userId);
    }
}
