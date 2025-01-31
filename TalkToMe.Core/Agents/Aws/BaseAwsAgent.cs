using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;
using TalkToMe.Domain.Enums;

namespace TalkToMe.Core.Agents.Aws;

public abstract class BaseAwsAgent : IAgent
{
    private IBedrockAgentService _bedrockAgentService;
    private IHistoryService _historyService;
    
    public abstract string AgentId { get; }
    
    protected BaseAwsAgent(IBedrockAgentService bedrockAgentService, IHistoryService historyService)
    {
        _bedrockAgentService = bedrockAgentService;
        _historyService = historyService;
    }

    protected async Task<CoreResponse> Invoke(CoreRequest request, string sessionId, string agentId, string agentAliasId)
    {
        var task1 = _historyService.SaveHistory(GetKey(sessionId), ChatRole.User, request.Prompt);
        var response = await _bedrockAgentService.Invoke(request.Prompt, sessionId, agentId, agentAliasId);
        var task2 = _historyService.SaveHistory(GetKey(sessionId), ChatRole.Assistant, response.Response);

        await Task.WhenAll(task1, task2);
        return response;
    }
    
    private string GetKey(string sessionId)
    {
        return $"{sessionId}{AgentId}";
    }

    public Task<CoreResponse> Invoke()
    {
        throw new NotImplementedException();
    }

    public Task<CoreResponse> Invoke(string promt)
    {
        throw new NotImplementedException();
    }

    public Task<CoreResponse> Invoke(string promt, string message)
    {
        throw new NotImplementedException();
    }
    
    public virtual Task<CoreResponse> InvokeWithSession(string sessionId)
    {
        throw new NotImplementedException();
    }

    public virtual Task<CoreResponse> InvokeWithSession(string promt, string sessionId)
    {
        throw new NotImplementedException();
    }
}