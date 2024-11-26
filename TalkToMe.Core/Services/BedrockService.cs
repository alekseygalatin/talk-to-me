using TalkToMe.Core.Configuration;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Services;

public class BedrockService : IAiService
{
    private readonly Dictionary<string, IAiModelService> _modelServices = new();

    public BedrockService(
        IBedrockClientFactory clientFactory, 
        BedrockSettings settings,
        IConversationManager conversationManager)
    {
        _modelServices = new Dictionary<string, IAiModelService>
        {
            {
                "us.meta.llama3-1-8b-instruct-v1:0",
                new LamaAiModelService(clientFactory, settings, conversationManager)
            }
        };
    }

    public async Task<CoreResponse> SendMessageAsync(CoreRequest request)
    {
        var aiModelService = _modelServices.GetValueOrDefault(request.ModelId)!;
        return await aiModelService.InvokeModelAsync(request);
    }
}