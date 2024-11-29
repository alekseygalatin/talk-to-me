using TalkToMe.Core.Services;

namespace TalkToMe.Core.Interfaces;

public interface IConversationManager
{
    Task AddMemory(string promt, List<Dialog> dialogs);
    Task<IEnumerable<Dialog>> GetMemories(string promt);
}