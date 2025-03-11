using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;
using TalkToMe.Core.Options;

namespace TalkToMe.Core.Agents.Aws;

public class ConversationSwedishAgent : BaseAwsAgent
{
    private AwsAgentOptions _awsAgentOptions;
    
    public ConversationSwedishAgent(
        IBedrockAgentService bedrockAgentService, 
        IHistoryService historyService, 
        IQueryCounterService queryCounterService, 
        AwsAgentOptions awsAgentOptions) : 
        base(bedrockAgentService, historyService, queryCounterService)
    {
        _awsAgentOptions = awsAgentOptions;
    }
    
    protected override string AwsAgentId => _awsAgentOptions.ConversationAgentSeId;
    protected override string AwsAliasId => _awsAgentOptions.ConversationAgentAliasSeId;
    
    public override string AgentId => "6a47a518-cc4b-4b40-b7a5-94113d8c1b16";
    
    public override async Task<CoreResponse> Invoke()
    {
        return await base.Invoke(new CoreRequest
        {
            Prompt = Message
        }, Session, AwsAgentId, AwsAliasId);
    }
}