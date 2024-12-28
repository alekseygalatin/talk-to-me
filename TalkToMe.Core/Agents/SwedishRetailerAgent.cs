using TalkToMe.Core.Builders;
using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents;

public class SwedishRetailerAgent : BaseAgent
{
    public SwedishRetailerAgent(IAIProviderFactory aiProviderFactory) :
        base(aiProviderFactory, AIProvider.AmazonBedrock, BedrockAIModelNames.Lama3_3_70b_v1)
    {
    }

    protected override string SystemPromt =>
        "Jag kommer att ge dig en originaltext och min återberättelse av den. Din uppgift är att enbart ställa frågor baserade på den angivna berättelsen och min återberättelse. Fokusera på att: Ställa klargörande frågor om detaljer som kan saknas, vara otydliga eller misstolkade i min återberättelse. Fråga om viktiga teman eller idéer från originaltexten som borde ha inkluderats. Ställa frågor som hjälper mig att reflektera djupare över textens innehåll och dess betydelse. Ställ enbart frågor relaterade till originaltexten och min återberättelse – inga andra kommentarer eller analyser behövs.";

    public async Task<CoreResponse> Invoke(string originalText, string retailing)
    {
        var promt = new StringBuilder(SystemPromt);
        promt.Append($"Här är originaltexten: {originalText}. ");
        
        var request = new CoreRequestBuilder()
        .WithSystemInstruction(promt.ToString())
        .WithPrompt($"Här är min återberättelse: {retailing}.")
        .Build();

        return await base.Invoke(request);
    }
}