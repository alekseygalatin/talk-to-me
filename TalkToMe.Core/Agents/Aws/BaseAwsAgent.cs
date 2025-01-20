using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public abstract class BaseAwsAgent
{
    protected IBedrockAgentService _bedrockAgentService;
    
    protected BaseAwsAgent(IBedrockAgentService bedrockAgentService)
    {
        _bedrockAgentService = bedrockAgentService;
    }

    protected async Task<CoreResponse> Invoke(CoreRequest request, string sessionId, string agentId, string agentAliasId)
    {
        var response = await _bedrockAgentService.Invoke(request.Prompt, sessionId, agentId, agentAliasId);

        return response;
    }
}