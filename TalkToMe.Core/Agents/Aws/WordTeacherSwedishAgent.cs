using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class WordTeacherSwedishAgent : BaseAwsAgent
{
    private readonly IWordService _wordService;
    
    public WordTeacherSwedishAgent(IBedrockAgentService bedrockAgentService, IWordService wordService, IHistoryService historyService) : 
        base(bedrockAgentService, historyService)
    {
        _wordService = wordService;
    }
    
    public override string AgentId => "f9b126e3-4340-4009-8b72-29a4ec321e7c";
    
    private string AwsAgentId = "DSXU3CSLY0";
    private string AwsAliasId = "MQPM7HUVK9";
    
    public override async Task<CoreResponse> Invoke()
    {
        if (string.IsNullOrWhiteSpace(Message))
        {
            var words = await _wordService.GetWords(Session, "sv-SE");
            var list = words.Select(x => x.Word);
            var str = new StringBuilder("Ställ endast frågor till mig för att bygga en dialog och hjälpa mig att lära mig orden ett i taget. Använd din minneshantering för att hålla koll på vilka ord som redan har täckts och vilka som inte har det, och håll svaren till högst två meningar, utan att inkludera något som inte är relaterat till ordlärandet. Här är orden jag vill öva på: ");
            str.Append(string.Join(", ", list));
            str.Append(".");
        
            return await base.Invoke(new CoreRequest
            {
                Prompt = str.ToString()
            }, Session, AwsAgentId, AwsAliasId);
        }

        return await base.Invoke(new CoreRequest
        {
            Prompt = Message
        }, Session, AwsAgentId, AwsAliasId);
    }
}