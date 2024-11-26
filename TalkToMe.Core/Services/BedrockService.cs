using TalkToMe.Core.Configuration;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Services;

public class AiService: IAiService
{
    private readonly Dictionary<string, IAiModelService> _modelServices = new();

    public AiService(
        IBedrockClientFactory clientFactory, 
        BedrockSettings settings,
        IConversationManager conversationManager)
    {
        _modelServices = new Dictionary<string, IAiModelService>
        {
            {
                "us.meta.llama3-1-8b-instruct-v1:0",
                new LamaAiModelService(clientFactory, settings, conversationManager)
            },
            {
                "amazon.titan-embed-text-v1",
                new TitanTextEmbedAiModelService(clientFactory, settings)
            }
        };
    }

    public async Task<CoreBedrockResponse> InvokeModelAsync(CoreBedrockRequest request)
    {
        var aiModelService = _modelServices.GetValueOrDefault(request.ModelId)!;
        return await aiModelService.InvokeModelAsync(request);
    }
}