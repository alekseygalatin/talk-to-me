using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class SwedishEmmaAgent : BaseAwsAgent
{
    private readonly IWordService _wordService;
    
    public SwedishEmmaAgent(IBedrockAgentService bedrockAgentService, IWordService wordService, IHistoryService historyService) : 
        base(bedrockAgentService, historyService)
    {
        _wordService = wordService;
    }

    public override async Task<CoreResponse> InvokeWithSession(string text, string sessionId)
    {
        return await base.Invoke(new CoreRequest
        {
            Prompt = text
        }, sessionId, "DSXU3CSLY0", "75Z4CXPDA9");
    }

    public override async Task<CoreResponse> InvokeWithSession(string sessionId)
    {
        var words = await _wordService.GetWords(sessionId, "sv-SE");
        var list = words.Select(x => x.Word);
        var str = new StringBuilder("Ställ endast frågor till mig för att bygga en dialog och hjälpa mig att lära mig orden ett i taget. Använd din minneshantering för att hålla koll på vilka ord som redan har täckts och vilka som inte har det, och håll svaren till högst två meningar, utan att inkludera något som inte är relaterat till ordlärandet. Här är orden jag vill öva på: ");
        str.Append(string.Join(", ", list));
        str.Append(".");
        
        return await base.Invoke(new CoreRequest
        {
            Prompt = str.ToString()
        }, sessionId, "DSXU3CSLY0", "75Z4CXPDA9");
    }
    
    public override string AgentId => "f9b126e3-4340-4009-8b72-29a4ec321e7c";
}