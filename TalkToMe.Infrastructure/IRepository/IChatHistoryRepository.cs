using TalkToMe.Domain.Entities;

namespace TalkToMe.Infrastructure.IRepository;

public interface IChatHistoryRepository : IBaseRepository<ChatHistoryEntity>
{
    Task<IList<ChatHistoryEntity>> GetChatHistoryAsync(string id);
}