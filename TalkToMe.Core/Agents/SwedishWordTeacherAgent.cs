using TalkToMe.Core.Builders;
using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents;

public class SwedishWordTeacherAgent : BaseWithMemoryAgent
{
    private readonly IWordService _wordService;
    
    public SwedishWordTeacherAgent(IAIProviderFactory aiProviderFactory, IConversationManager conversationManager, IWordService wordService) :
        base(aiProviderFactory, conversationManager, AIProvider.AmazonBedrock, BedrockAIModelNames.Claude_3_5_Haiku)
    {
        _wordService = wordService;
    }
    
    protected override string SystemPromt => "";
    protected override string AgentId => "6";

    public async Task<CoreResponse> Invoke(string message, string sessionId)
    {
        var promt = await BuildSystemPromt(message, sessionId);
        
        var request = new CoreRequestBuilder()
        .WithSystemInstruction(promt)
        .WithPrompt(message)
        .Build();

        return await base.Invoke(request, sessionId);
    }

    protected override async Task<string> BuildSystemPromt(string message, string sessionId)
    {
        var words = await _wordService.GetWords(sessionId);
        var list = words.Select(x => x.Word);
        var str = new StringBuilder("Jag lär mig svenska och vill att du ska hjälpa mig som en språkinlärningspartner för att träna följande ord: ");
        str.Append(string.Join(", ", list));
        str.Append(".");
        str.Append(
            "Bygg dialoger med mig där du använder dessa ord naturligt och utnyttjar historiken för att säkerställa att alla ord täcks. Ställ frågor som hjälper mig att memorera dem och be mig använda orden i egna meningar. Fokusera på att skapa en logisk och engagerande konversation kring orden, där varje fråga eller svar bygger vidare på tidigare interaktion. Du behöver inte fokusera på ett ord i taget, men använd orden så ofta som möjligt. Målet är att lära mig alla ord genom aktiv interaktion. Prata endast svenska och håll varje svar till högst två meningar för att hålla dialogen dynamisk.");
        
        var memory = await base.BuildSystemPromt(message, sessionId);
        str.Append(memory);

        return str.ToString();
    }
}