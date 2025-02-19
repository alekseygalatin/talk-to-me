using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents;

public abstract class BaseAgent : IAgent
{
    private IAiModelService _model;
    private IAgent _agentImplementation;

    protected abstract string SystemPromt { get; }
    
    protected string Promt { get; set; }
    protected string Message { get; set; }
    protected string Session { get; set; }

    protected BaseAgent(IAIProviderFactory aiProviderFactory, AIProvider aiProvider, string model)
    {
        var provider = aiProviderFactory.GetProvider(aiProvider);
        _model = provider.GetModel(model);
    }
    
    protected async Task<CoreResponse> Invoke(CoreRequest request)
    {
        return await _model.SendMessageAsync(request);
    }

    protected async Task<string> BuildSystemPromt()
    {
        return await Task.FromResult(SystemPromt);
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