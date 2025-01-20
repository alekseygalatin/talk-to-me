using TalkToMe.Core.Models;

namespace TalkToMe.Core.Interfaces;

public interface IBedrockAgentService
{
    Task<CoreResponse> Invoke(string input, string sessionId, string agentId, string agentAliasId);
}