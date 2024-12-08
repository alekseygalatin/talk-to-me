using TalkToMe.Core.Builders;
using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents;

public class SwedishStoryTailorAgent : BaseAgent
{
    public SwedishStoryTailorAgent(IAIProviderFactory aiProviderFactory) :
        base(aiProviderFactory, AIProvider.AmazonBedrock, BedrockAIModelNames.Lama3_1_70b_v1)
    {
    }
    
    protected override string SystemPromt => "Du är en vänlig sagoberättare som heter Maria. Skriv en kort berättelse på mycket enkel svenska. Berättelsen ska vara lätt att läsa och passa för en person som lär sig svenska. Använd korta meningar, vanliga ord och fokusera på ett intressant eller roligt tema. Berättelsen ska vara cirka 150–200 ord lång. Undvik svåra ord och komplicerad grammatik. Efter berättelsen, ställ en enkel och allmän fråga om berättelsen så att läsaren kan återberätta den med egna ord.";

    public async Task<CoreResponse> Invoke()
    {
        var promt = await BuildSystemPromt();
        
        var request = new CoreRequestBuilder()
        .WithSystemInstruction(promt)
        .Build();

        return await base.Invoke(request);
    }
}