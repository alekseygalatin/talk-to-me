using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;
using TalkToMe.Core.Options;

namespace TalkToMe.Core.Agents.Aws;

public class StoryRetailerSwedishAgent : BaseAwsAgent
{
    private AwsAgentOptions _awsAgentOptions;
    
    public StoryRetailerSwedishAgent(
        IBedrockAgentService bedrockAgentService, 
        IHistoryService historyService, 
        IQueryCounterService queryCounterService, 
        AwsAgentOptions awsAgentOptions) : 
        base(bedrockAgentService, historyService, queryCounterService)
    {
        _awsAgentOptions = awsAgentOptions;
    }
    
    public override string AgentId => "6e83c910-d4c2-491f-94ce-cd8f2c468c43";
    
    protected override string AwsAgentId => _awsAgentOptions.StoryRetailerAgentSeId;
    protected override string AwsAliasId => _awsAgentOptions.StoryRetailerAgentAliasSeId;
    
    public override async Task<CoreResponse> Invoke()
    {
        return await base.Invoke(new CoreRequest
        {
            Prompt = string.IsNullOrEmpty(Message) ? $"Här är originaltexten vi ska arbeta med: {Promt}" : $"Här är min återberättelse: {Message}"
        }, Session, AwsAgentId, AwsAliasId);
    }
}