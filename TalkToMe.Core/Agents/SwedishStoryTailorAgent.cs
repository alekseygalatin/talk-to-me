using TalkToMe.Core.Builders;
using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents;

public class SwedishStoryTailorAgent : BaseAgent
{
    private SwedishRetailerAgent _swedishRetailer;
    public SwedishStoryTailorAgent(IAIProviderFactory aiProviderFactory, IQueryCounterService queryCounterService) :
        base(aiProviderFactory, queryCounterService, AIProvider.AmazonBedrock, BedrockAIModelNames.Claude_3_5_Haiku)
    {
        _swedishRetailer = new SwedishRetailerAgent(aiProviderFactory, queryCounterService);
    }
    
    protected override string SystemPromt => "Du är en vänlig sagoberättare som heter Maria. Skriv en kort berättelse på mycket enkel svenska. Berättelsen ska vara lätt att läsa och passa för en person som lär sig svenska. Använd korta meningar, vanliga ord och fokusera på ett intressant eller roligt tema. Berättelsen ska vara cirka 150–200 ord lång. Undvik svåra ord och komplicerad grammatik. Efter berättelsen, ställ en enkel och allmän fråga om berättelsen så att läsaren kan återberätta den med egna ord.";

    public override async Task<CoreResponse> Invoke()
    {
        if (string.IsNullOrWhiteSpace(Message))
        {
            var promt = await BuildSystemPromt();
        
            var request = new CoreRequestBuilder()
                .WithSystemInstruction(promt)
                .WithPrompt("skriv")
                .Build();

            return await base.Invoke(request, Session);
        }
        
        return await _swedishRetailer.WithPromt(Promt).WithMessage(Message).Invoke();
    }
}