using TalkToMe.Core.Models;

namespace TalkToMe.Core.Builders;

public class BedrockRequestBuilder
{
    private readonly CoreBedrockRequest _request = new();

    public BedrockRequestBuilder WithPrompt(string prompt)
    {
        _request.Prompt = prompt;
        return this;
    }

    public BedrockRequestBuilder WithModel(string modelId)
    {
        _request.ModelId = modelId;
        return this;
    }

    public BedrockRequestBuilder WithSystemInstruction(string instruction)
    {
        _request.SystemInstruction = instruction;
        return this;
    }
    
    public BedrockRequestBuilder WithHistory()
    {
        _request.SupportHistory = true;
        return this;
    }


    public CoreBedrockRequest Build() => _request;
}