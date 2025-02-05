using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;
using TalkToMe.Domain.Enums;

namespace TalkToMe.Core.Agents.Aws;

public abstract class BaseAwsAgent : IAgent
{
    private IBedrockAgentService _bedrockAgentService;
    private IHistoryService _historyService;
    
    public abstract string AgentId { get; }
    
    protected string Promt { get; set; }
    protected string Message { get; set; }
    protected string Session { get; set; }
    
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

    public IAgent WithPromt(string promt)
    {
        Promt = promt;
        return this;
    }

    public IAgent WithMessage(string message)
    {
        Message = message;
        return this;
    }

    public IAgent WithSession(string sessionId)
    {
        Session = sessionId;
        return this;
    }

    public abstract Task<CoreResponse> Invoke();
}