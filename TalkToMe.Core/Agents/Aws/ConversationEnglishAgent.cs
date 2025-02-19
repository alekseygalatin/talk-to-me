using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class ConversationEnglishAgent : BaseAwsAgent
{
    public ConversationEnglishAgent(IBedrockAgentService bedrockAgentService, IHistoryService historyService) : 
        base(bedrockAgentService, historyService)
    { 
    }
    
    private string AwsAgentId = "63O8MUCFBG";
    private string AwsAliasId = "QOJMBGO5LW";

    public override string AgentId => "7b542e6e-7e2a-424f-8e76-0fec63cb2568";
    
    public override async Task<CoreResponse> Invoke()
    {
        return await base.Invoke(new CoreRequest
        {
            Prompt = Message
        }, Session, AwsAgentId, AwsAliasId);
    }
}