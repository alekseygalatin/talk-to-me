using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class StoryRetailerEnglishAgent : BaseAwsAgent
{
    public StoryRetailerEnglishAgent(IBedrockAgentService bedrockAgentService, IHistoryService historyService, IQueryCounterService queryCounterService) : 
        base(bedrockAgentService, historyService, queryCounterService)
    {
    }
    
    public override string AgentId => "a7568025-1326-493b-9c2d-070255881e08";
    
    protected override string AwsAgentId => "DLHYW6ZCYM";
    protected override string AwsAliasId => "EPQD5EPOV5";
    
    public override async Task<CoreResponse> Invoke()
    {
        var promt = new StringBuilder();
        promt.Append($"This is original text: {Promt}. ");
        promt.Append($"This is my retailing: {Message}.");
        
        return await base.Invoke(new CoreRequest
        {
            Prompt = promt.ToString()
            
        }, Session, AwsAgentId, AwsAliasId);
    }
}