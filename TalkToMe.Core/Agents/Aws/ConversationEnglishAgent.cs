using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;
using TalkToMe.Core.Options;

namespace TalkToMe.Core.Agents.Aws;

public class ConversationEnglishAgent : BaseAwsAgent
{
    private AwsAgentOptions _awsAgentOptions;
    
    public ConversationEnglishAgent(
        IBedrockAgentService bedrockAgentService, 
        IHistoryService historyService, 
        IQueryCounterService queryCounterService,
        AwsAgentOptions awsAgentOptions) : 
        base(bedrockAgentService, historyService, queryCounterService)
    {
        _awsAgentOptions = awsAgentOptions;
    }

    protected override string AwsAgentId => _awsAgentOptions.ConversationAgentEnId;
    protected override string AwsAliasId => _awsAgentOptions.ConversationAgentAliasEnId;

    public override string AgentId => "7b542e6e-7e2a-424f-8e76-0fec63cb2568";
    
    public override async Task<CoreResponse> Invoke()
    {
        return await base.Invoke(new CoreRequest
        {
            Prompt = Message
        }, Session, AwsAgentId, AwsAliasId);
    }
}