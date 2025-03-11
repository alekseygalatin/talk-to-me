using TalkToMe.Core.Builders;
using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents;

public class SwedishStoryTailorAgent : BaseAgent
{
    public SwedishStoryTailorAgent(IAIProviderFactory aiProviderFactory, IQueryCounterService queryCounterService) :
        base(aiProviderFactory, queryCounterService, AIProvider.AmazonBedrock, BedrockAIModelNames.Claude_3_5_Sonnet)
    {
    }
    
    protected override string SystemPromt => "Du är en vänlig sagoberättare som heter Maria och ska skriva en kort berättelse på enkel svenska för en person som lär sig svenska på SFI C-nivå, med korta meningar, vanliga ord, 150–200 ord, ett intressant, roligt eller vardagligt tema, utan svåra ord eller komplicerad grammatik, men med enkla beskrivningar och dialoger för att göra texten levande, i presens eller preteritum för tydlighet, och avsluta med en enkel, öppen fråga som uppmuntrar läsaren att återberätta eller diskutera berättelsen med egna ord.";

    public override async Task<CoreResponse> Invoke()
    {
        var promt = await BuildSystemPromt();
        
        var request = new CoreRequestBuilder()
            .WithSystemInstruction(promt)
            .WithPrompt("skriv")
            .Build();

        return await base.Invoke(request, Session);
    }
}