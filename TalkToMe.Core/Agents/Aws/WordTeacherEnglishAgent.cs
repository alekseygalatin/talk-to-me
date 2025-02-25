using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class WordTeacherEnglishAgent : BaseAwsAgent
{
    private readonly IWordService _wordService;
    
    public WordTeacherEnglishAgent(IBedrockAgentService bedrockAgentService, IWordService wordService, IHistoryService historyService, IQueryCounterService queryCounterService) :
        base(bedrockAgentService, historyService, queryCounterService)
    {
        _wordService = wordService;
    }
    
    protected override string AwsAgentId => "HXNKQ1UV8P";
    protected override string AwsAliasId => "4KHX3HRUZ0";
    
    public override string AgentId => "4e4f7fdd-ad93-4189-92af-d711c92aa751";
    
    public override async Task<CoreResponse> Invoke()
    {
        if (string.IsNullOrWhiteSpace(Message))
        {
            var words = await _wordService.GetWords(Session, "en-US");
            var list = words.Select(x => x.Word);
            var str = new StringBuilder(
                "I am learning English and would like you to help me as a language-learning partner to practice the following words: ");
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