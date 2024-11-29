using TalkToMe.Core.Services;

namespace TalkToMe.Core.Interfaces;

public interface IConversationManager
{
    Task AddMemory(string promt, List<Dialog> dialogs, string sessionId);
    Task<IEnumerable<Dialog>> GetMemories(string promt, string sessionId);
}