using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;
using TalkToMe.Core.Services;

namespace TalkToMe.Core.Agents;

public abstract class BaseWithMemoryAgent
{
    protected IAiModelService _model;
    private IConversationManager _conversationManager;
    
    protected abstract string SystemPromt { get; }

    protected BaseWithMemoryAgent(IAIProviderFactory aiProviderFactory, IConversationManager conversationManager, AIProvider aiProvider, string model)
    {
        var provider = aiProviderFactory.GetProvider(aiProvider);
        _model = provider.GetModel(model);
        _conversationManager = conversationManager;
    }
    
    protected async Task<CoreResponse> Invoke(CoreRequest request, string sessionId)
    {
        var response = await _model.SendMessageAsync(request);
        
        await _conversationManager.AddMemory($"{request.Prompt}. {response.Response}", new List<Dialog>
        {
            new Dialog
            {
                Role = "user",
                Message = request.Prompt
            },
            new Dialog
            {
                Role = "model",
                Message = response.Response
            }
        }, sessionId);

        return response;
    }

    protected async Task<string> BuildSystemPromt(string message, string sessionId)
    {
        var promptBuilder = new StringBuilder();
        
        promptBuilder.AppendLine(SystemPromt);
        promptBuilder.AppendLine("The following section contains the conversation history for your reference. Do not include or repeat this history in your response. Only respond to the user's latest input after the conversation history:");
        var memories = await _conversationManager.GetMemories(message, sessionId);
        foreach (var memory in memories)
        {
            promptBuilder.AppendLine($"{memory.Role}: {memory.Message}");
        }

        return promptBuilder.ToString();
    }
}