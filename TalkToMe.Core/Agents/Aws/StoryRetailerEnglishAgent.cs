using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;
using TalkToMe.Core.Options;

namespace TalkToMe.Core.Agents.Aws;

public class StoryRetailerEnglishAgent : BaseAwsAgent
{
    private AwsAgentOptions _awsAgentOptions;
    
    public StoryRetailerEnglishAgent(
        IBedrockAgentService bedrockAgentService, 
        IHistoryService historyService,
        IQueryCounterService queryCounterService, 
        AwsAgentOptions awsAgentOptions) : 
        base(bedrockAgentService, historyService, queryCounterService)
    {
        _awsAgentOptions = awsAgentOptions;
    }
    
    public override string AgentId => "a7568025-1326-493b-9c2d-070255881e08";
    
    protected override string AwsAgentId => _awsAgentOptions.StoryRetailerAgentEnId;
    protected override string AwsAliasId => _awsAgentOptions.StoryRetailerAgentAliasEnId;
    
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