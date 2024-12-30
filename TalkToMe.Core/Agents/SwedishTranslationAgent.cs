using TalkToMe.Core.Builders;
using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents;

public class SwedishTranslationAgent : BaseTranslationAgent
{
    public SwedishTranslationAgent(IAIProviderFactory aiProviderFactory) :
        base(aiProviderFactory, AIProvider.AmazonBedrock, BedrockAIModelNames.Lama3_3_70b_v1)
    {
    }

    //protected override string SystemPromt => "You are a Swedish-to-English language translation agent. When given a Swedish word or phrase.\nProvide an accurate English translation, a brief example sentence showing natural usage in Swedish, and any relevant notes on nuances like article usage in Swedish.\n\nYour respons always must contain only JSON in the format: {\"translation\":  \"english translation\",  \"example_usage\": \"example swedish sentence\",  \"translation_notes\": \"notes on use\"}";
    protected override string SystemPromt => GetTranslationAgentPrompt("Swedish", "English");

    public async Task<CoreResponse> Invoke(string message)
    {
        var promt = await BuildSystemPromt();
        
        var request = new CoreRequestBuilder()
        .WithSystemInstruction(promt)
        .WithPrompt(message)
        .Build();

        return await base.Invoke(request);
    }
}