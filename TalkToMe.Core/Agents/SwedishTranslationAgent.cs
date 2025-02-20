using TalkToMe.Core.Builders;
using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents;

public class SwedishTranslationAgent : BaseTranslationAgent
{
    public SwedishTranslationAgent(IAIProviderFactory aiProviderFactory, IQueryCounterService queryCounterService) :
        base(aiProviderFactory, queryCounterService, AIProvider.AmazonBedrock, BedrockAIModelNames.Claude_3_5_Haiku)
    {
    }
    
    protected override string SystemPromt => GetTranslationAgentPrompt("Swedish", "English");
    
    public override async Task<CoreResponse> Invoke()
    {
        var promt = await BuildSystemPromt();
        
        var request = new CoreRequestBuilder()
            .WithSystemInstruction(promt)
            .WithPrompt(Message)
            .Build();

        return await base.Invoke(request, Session);
    }
}