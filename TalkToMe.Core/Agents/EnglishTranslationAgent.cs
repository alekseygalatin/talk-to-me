using TalkToMe.Core.Builders;
using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents;

public class EnglishTranslationAgent : BaseAgent
{
    public EnglishTranslationAgent(IAIProviderFactory aiProviderFactory) :
        base(aiProviderFactory, AIProvider.AmazonBedrock, BedrockAIModelNames.Lama3_3_70b_v1)
    {
    }
    
    protected override string SystemPromt => "You are a English-to-Russian language translation agent. When given a English word or phrase. Provide an accurate Russian translation, a brief example sentence showing natural usage in English, and any relevant notes on nuances like article usage in English. Your respons always must contain only JSON in the format: {\"translation\":  \"russian translation\",  \"example_usage\": \"example english sentence\",  \"translation_notes\": \"notes on use\"}";

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