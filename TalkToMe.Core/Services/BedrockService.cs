using TalkToMe.Core.Constants;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Core.Services;

public class BedrockService : IAIProvider
{
    private readonly Dictionary<string, IAiModelService> _modelServices = new();

    public BedrockService(
        IBedrockClientFactory clientFactory)
    {
        _modelServices = new Dictionary<string, IAiModelService>
        {
            {
                BedrockAIModelNames.Lama3_1_8b_v1, 
                new LamaAiModelService(clientFactory, BedrockAIModelNames.Lama3_1_8b_v1)
            },
            {
                BedrockAIModelNames.Lama3_1_70b_v1, 
                new LamaAiModelService(clientFactory, BedrockAIModelNames.Lama3_1_70b_v1)
            },
            {
                BedrockAIModelNames.AWS_Nova_Pro,
                new NovaProModelService(clientFactory, BedrockAIModelNames.AWS_Nova_Pro)
            },
            {
                BedrockAIModelNames.Claude_3_5_Haiku,
                new ClaudeHaikuModelService(clientFactory, BedrockAIModelNames.Claude_3_5_Haiku)
            }
        };
    }

    public IAiModelService GetModel(string modelId)
    {
        var aiModel = _modelServices.GetValueOrDefault(modelId);

        if (aiModel is null)
            throw new NotSupportedException($"Model with id {modelId} is not suported");

        return aiModel;
    }
   
}