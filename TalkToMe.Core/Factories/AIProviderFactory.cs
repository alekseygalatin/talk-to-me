using Microsoft.Extensions.DependencyInjection;
using TalkToMe.Core.Configuration;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Services;

namespace TalkToMe.Core.Factories
{
    public class AIProviderFactory : IAIProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public AIProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IAIProvider GetProvider(AIProvider provider)
        {
            return provider switch
            {
                AIProvider.AmazonBedrock => _serviceProvider.GetRequiredService<BedrockService>(),
                AIProvider.OpenAI => _serviceProvider.GetRequiredService<OpenAiService>(),
                _ => throw new NotSupportedException($"Provider is not supported.")
            };
        }
    }
}
