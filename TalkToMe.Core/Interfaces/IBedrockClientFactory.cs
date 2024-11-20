namespace TalkToMe.Core.Interfaces;

public interface IBedrockClientFactory
{
    AmazonBedrockRuntimeClient CreateClient();
}