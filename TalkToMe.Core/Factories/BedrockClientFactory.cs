using Amazon.BedrockRuntime;
using Amazon.Runtime;
using System;
using Amazon.BedrockAgentRuntime;
using Microsoft.Extensions.Options;
using TalkToMe.Core.Configuration;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Core.Factories
{
    public class BedrockClientFactory : IBedrockClientFactory
    {
        private readonly BedrockSettings _settings;

        public BedrockClientFactory(BedrockSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public AmazonBedrockRuntimeClient CreateClient()
        {
            return new AmazonBedrockRuntimeClient(
                new AmazonBedrockRuntimeConfig
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(_settings.Region)
                });
        }

        public AmazonBedrockAgentRuntimeClient CreateAgentClient()
        {
            return new AmazonBedrockAgentRuntimeClient(
                new AmazonBedrockAgentRuntimeConfig
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(_settings.Region)
                });
        }
    }
} 