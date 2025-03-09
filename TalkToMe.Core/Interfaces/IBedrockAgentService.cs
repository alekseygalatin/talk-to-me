using TalkToMe.Core.Models;

namespace TalkToMe.Core.Interfaces;

public interface IBedrockAgentService
{
    Task<CoreResponse> Invoke(string input, string sessionId, string agentId, string agentAliasId);
    Task CleanMemory(string sessionId, string agentId, string agentAliasId);
}