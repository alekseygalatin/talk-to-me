using TalkToMe.Core.Models;

namespace TalkToMe.Core.Interfaces;

public interface IAgent
{
    Task<CoreResponse> Invoke();
    Task<CoreResponse> Invoke(string promt);
    Task<CoreResponse> Invoke(string promt, string message);
    Task<CoreResponse> InvokeWithSession(string sessionId);
    Task<CoreResponse> InvokeWithSession(string promt, string sessionId);
}