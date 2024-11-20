public class BedrockRequestBuilder
{
    private readonly BedrockRequest _request = new();

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

    public BedrockRequest Build() => _request;
} 