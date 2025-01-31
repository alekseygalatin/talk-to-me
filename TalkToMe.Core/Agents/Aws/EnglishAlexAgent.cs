using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class EnglishAlexAgent : BaseAwsAgent
{
    public EnglishAlexAgent(IBedrockAgentService bedrockAgentService, IHistoryService historyService) : 
        base(bedrockAgentService, historyService)
    { 
    }

    public async Task<CoreResponse> InvokeWithSession(string text, string sessionId)
    {
        return await base.Invoke(new CoreRequest
        {
            Prompt = text
        }, sessionId, "63O8MUCFBG", "QOJMBGO5LW");
    }

    public override string AgentId => "7b542e6e-7e2a-424f-8e76-0fec63cb2568";
}