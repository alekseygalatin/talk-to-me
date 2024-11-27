using TalkToMe.Core.Configuration;
using TalkToMe.Core.Constants;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Core.Services
{
    public class OpenAiService : IAIProvider
    {
        private readonly Dictionary<string, IAiModelService> _modelServices = new();

        public OpenAiService(OpenAiSettings settings)
        {
            _modelServices = new Dictionary<string, IAiModelService>
            {
                {
                    OpenAIModelNames.GPT4oMini, new ChatGPT4oMiniModelService(settings, OpenAIModelNames.GPT4oMini)
                },
                {
                    OpenAIModelNames.GPT4o, new ChatGPT4oModelService(settings, OpenAIModelNames.GPT4o)
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
}
