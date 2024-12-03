using TalkToMe.Core.Models;

namespace TalkToMe.Core.Builders;

public class CoreRequestBuilder
{
    private readonly CoreRequest _request = new();

    public CoreRequestBuilder WithPrompt(string prompt)
    {
        _request.Prompt = prompt;
        return this;
    }

    public CoreRequestBuilder WithSystemInstruction(string instruction)
    {
        _request.SystemInstruction = instruction;
        return this;
    }

    public CoreRequest Build() => _request;
}