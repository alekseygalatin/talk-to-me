using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;
using TalkToMe.Core.Services;

namespace TalkToMe.Core.Agents.Aws;

public abstract class BaseAwsAgent
{
    protected IBedrockAgentService _bedrockAgentService;
    private IConversationManager _conversationManager;

    protected abstract string AgentId { get; }
    
    protected BaseAwsAgent(IBedrockAgentService bedrockAgentService, IConversationManager conversationManager)
    {
        _bedrockAgentService = bedrockAgentService;
        _conversationManager = conversationManager;
    }

    protected async Task<CoreResponse> Invoke(CoreRequest request, string sessionId, string agentId, string agentAliasId)
    {
        var task1 =  _conversationManager.AddMemory($"{request.Prompt}", new List<Dialog>
        {
            new Dialog
            {
                Role = "user",
                Message = request.Prompt
            }
        }, GetKey(sessionId));
        
        var response = await _bedrockAgentService.Invoke(request.Prompt, sessionId, agentId, agentAliasId);
        
        var task2 = _conversationManager.AddMemory($"{response.Response}", new List<Dialog>
        {
            new Dialog
            {
                Role = "model",
                Message = response.Response
            }
        }, GetKey(sessionId));

        await Task.WhenAll(task1, task2);

        return response;
    }
    
    private string GetKey(string sessionId)
    {
        return $"{sessionId}{AgentId}";
    }
}