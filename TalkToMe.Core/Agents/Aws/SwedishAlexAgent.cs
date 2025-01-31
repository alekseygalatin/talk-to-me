using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class SwedishAlexAgent : BaseAwsAgent
{
    public SwedishAlexAgent(IBedrockAgentService bedrockAgentService, IHistoryService historyService) : 
        base(bedrockAgentService, historyService)
    {
    }

    public override async Task<CoreResponse> InvokeWithSession(string text, string sessionId)
    {
        return await base.Invoke(new CoreRequest
        {
            Prompt = text
        }, sessionId, "NWZQ7VJKHG", "D8OT4ASWHD");
    }
    
    public override string AgentId => "6a47a518-cc4b-4b40-b7a5-94113d8c1b16";
}