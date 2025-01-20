using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class EnglishEmmaAgent : BaseAwsAgent
{
    private readonly IWordService _wordService;
    
    public EnglishEmmaAgent(IBedrockAgentService bedrockAgentService, IConversationManager conversationManager, IWordService wordService) : base(bedrockAgentService, conversationManager)
    {
        _wordService = wordService;
    }

    public async Task<CoreResponse> Invoke(string text, string sessionId)
    {
        return await base.Invoke(new CoreRequest
        {
            Prompt = text
        }, sessionId, "HXNKQ1UV8P", "J5YBQDNSPB");
    }

    public async Task<CoreResponse> InitialInvoke(string sessionId)
    {
        var words = await _wordService.GetWords(sessionId, "en-US");
        var list = words.Select(x => x.Word);
        var str = new StringBuilder("I am learning English and would like you to help me as a language-learning partner to practice the following words: ");
        str.Append(string.Join(", ", list));
        str.Append(".");
        
        return await base.Invoke(new CoreRequest
        {
            Prompt = str.ToString()
        }, sessionId, "HXNKQ1UV8P", "J5YBQDNSPB");
    }

    protected override string AgentId => "05e4fb8c-cfc2-45b3-adf4-12e7d119bfcd";
}