using TalkToMe.Core.Models;
using TalkToMe.Domain.Enums;

namespace TalkToMe.Core.Interfaces;

public interface IHistoryService
{
    Task<IEnumerable<MessageModel>> GetHistory(string sessionId);
    Task SaveHistory(string sessionId, ChatRole role, string message);
    Task CleanAgentHistory(string sessionId);
}