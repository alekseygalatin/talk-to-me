using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class ConversationSwedishAgent : BaseAwsAgent
{
    public ConversationSwedishAgent(IBedrockAgentService bedrockAgentService, IHistoryService historyService, IQueryCounterService queryCounterService) : 
        base(bedrockAgentService, historyService, queryCounterService)
    {
    }
    
    protected override string AwsAgentId => "NWZQ7VJKHG";
    protected override string AwsAliasId => "D8OT4ASWHD";
    
    public override string AgentId => "6a47a518-cc4b-4b40-b7a5-94113d8c1b16";
    
    public override async Task<CoreResponse> Invoke()
    {
        return await base.Invoke(new CoreRequest
        {
            Prompt = Message
        }, Session, AwsAgentId, AwsAliasId);
    }
}