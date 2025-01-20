using Amazon.BedrockAgentRuntime;

namespace TalkToMe.Core.Interfaces;

public interface IBedrockClientFactory
{
    AmazonBedrockRuntimeClient CreateClient();
    AmazonBedrockAgentRuntimeClient CreateAgentClient();
}