using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class EnglishEmmaAgent : BaseAwsAgent
{
    private readonly IWordService _wordService;
    
    public EnglishEmmaAgent(IBedrockAgentService bedrockAgentService, IWordService wordService, IHistoryService historyService) :
        base(bedrockAgentService, historyService)
    {
        _wordService = wordService;
    }

    public override async Task<CoreResponse> InvokeWithSession(string text, string sessionId)
    {
        return await base.Invoke(new CoreRequest
        {
            Prompt = text
        }, sessionId, "HXNKQ1UV8P", "4KHX3HRUZ0");
    }

    public override async Task<CoreResponse> InvokeWithSession(string sessionId)
    {
        var words = await _wordService.GetWords(sessionId, "en-US");
        var list = words.Select(x => x.Word);
        var str = new StringBuilder("I am learning English and would like you to help me as a language-learning partner to practice the following words: ");
        str.Append(string.Join(", ", list));
        str.Append(".");
        
        return await base.Invoke(new CoreRequest
        {
            Prompt = str.ToString()
        }, sessionId, "HXNKQ1UV8P", "4KHX3HRUZ0");
    }
    
    public override string AgentId => "4e4f7fdd-ad93-4189-92af-d711c92aa751";
}