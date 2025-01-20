using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class SwedishEmmaAgent : BaseAwsAgent
{
    private readonly IWordService _wordService;
    
    public SwedishEmmaAgent(IBedrockAgentService bedrockAgentService, IConversationManager conversationManager, IWordService wordService) : base(bedrockAgentService, conversationManager)
    {
        _wordService = wordService;
    }

    public async Task<CoreResponse> Invoke(string text, string sessionId)
    {
        return await base.Invoke(new CoreRequest
        {
            Prompt = text
        }, sessionId, "DSXU3CSLY0", "LQSILKO1TO");
    }

    public async Task<CoreResponse> InitialInvoke(string sessionId)
    {
        var words = await _wordService.GetWords(sessionId, "sv-SE");
        var list = words.Select(x => x.Word);
        var str = new StringBuilder("Jag lär mig svenska och vill att du ska hjälpa mig som en språkinlärningspartner för att träna följande ord: ");
        str.Append(string.Join(", ", list));
        str.Append(".");
        
        return await base.Invoke(new CoreRequest
        {
            Prompt = str.ToString()
        }, sessionId, "DSXU3CSLY0", "LQSILKO1TO");
    }

    protected override string AgentId => "73802da4-582f-4e22-8668-f4dfc3c87218";
}