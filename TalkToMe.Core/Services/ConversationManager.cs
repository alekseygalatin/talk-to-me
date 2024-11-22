using System.Threading.Tasks.Dataflow;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Core.Services;

public class ConversationManager : IConversationManager
{
    private readonly List<(string Role, string Content)> _conversationHistory = new();

    public void AddMessage(string role, string content)
    {
        _conversationHistory.Add((role, content));
    }

    public void GetFormattedPrompt(Action<string, string> buildFormatedPromt)
    {
        foreach (var (role, content) in _conversationHistory)
        {
            buildFormatedPromt(role, content);
        }
    }
}