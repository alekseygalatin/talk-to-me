using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;
using TalkToMe.Domain.Enums;

namespace TalkToMe.Core.Agents.Aws;

public abstract class BaseAwsAgent : IAgent
{
    protected IBedrockAgentService _bedrockAgentService;
    protected IHistoryService _historyService;
    private IQueryCounterService _queryCounterService;
    
    public abstract string AgentId { get; }
    
    protected abstract string AwsAgentId { get; }
    protected abstract string AwsAliasId { get; }
    protected string Promt { get; set; }
    protected string Message { get; set; }
    protected string Session { get; set; }
    
    protected BaseAwsAgent(IBedrockAgentService bedrockAgentService, IHistoryService historyService, IQueryCounterService queryCounterService)
    {
        _bedrockAgentService = bedrockAgentService;
        _historyService = historyService;
        _queryCounterService = queryCounterService;
    }

    protected async Task<CoreResponse> Invoke(CoreRequest request, string sessionId, string agentId, string agentAliasId, bool skipHistory = false)
    {
        await _queryCounterService.CheckLimitOrThrowError(sessionId);
        
        if (!skipHistory)
            await _historyService.SaveHistory(GetKey(sessionId), ChatRole.User, request.Prompt);
        
        var response = await _bedrockAgentService.Invoke(request.Prompt, sessionId, agentId, agentAliasId);
        
        if (!skipHistory)
            await _historyService.SaveHistory(GetKey(sessionId), ChatRole.Assistant, response.Response);
        
        return response;
    }
    
    protected async Task AddHistory(string sessionId, string message, ChatRole role)
    {
        await _historyService.SaveHistory(GetKey(sessionId), role, message);
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
    
    public async Task CleanMemory()
    {
        await _bedrockAgentService.CleanMemory(Session, AwsAgentId, AwsAliasId);
        await _historyService.CleanAgentHistory(Session + AgentId);
    }
}