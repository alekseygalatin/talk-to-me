using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents;

public abstract class BaseAgent : IAgent
{
    private IAiModelService _model;
    
    protected abstract string SystemPromt { get; }

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

    public virtual Task<CoreResponse> Invoke()
    {
        throw new NotImplementedException();
    }

    public virtual Task<CoreResponse> Invoke(string promt)
    {
        throw new NotImplementedException();
    }

    public virtual Task<CoreResponse> Invoke(string promt, string message)
    {
        throw new NotImplementedException();
    }

    public Task<CoreResponse> InvokeWithSession(string sessionId)
    {
        throw new NotImplementedException();
    }

    public Task<CoreResponse> InvokeWithSession(string promt, string sessionId)
    {
        throw new NotImplementedException();
    }
}