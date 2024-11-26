using TalkToMe.Core.Services;

namespace TalkToMe.Core.Interfaces;

public interface IConversationManager
{
    Task AddMessage(string content, List<Dialog> dialogs);
    Task<IEnumerable<string>> GetFormattedPrompt(string promt, Action<string, string> buildFormatedPromt);
}