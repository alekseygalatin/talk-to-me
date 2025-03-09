using TalkToMe.Core.Models;

namespace TalkToMe.Core.Interfaces;

public interface IAgent
{
    IAgent WithPromt(string promt);
    IAgent WithMessage(string message);
    IAgent WithSession(string sessionId);
    Task<CoreResponse> Invoke();
    Task CleanMemory();
}