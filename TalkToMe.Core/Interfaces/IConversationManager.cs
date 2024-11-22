namespace TalkToMe.Core.Interfaces;

public interface IConversationManager
{
    void AddMessage(string role, string content);
    void GetFormattedPrompt(Action<string, string> buildFormatedPromt);
}