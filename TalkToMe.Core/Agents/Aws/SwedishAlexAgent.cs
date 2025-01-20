using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class SwedishAlexAgent : BaseAwsAgent
{
    public SwedishAlexAgent(IBedrockAgentService bedrockAgentService, IConversationManager conversationManager) : base(bedrockAgentService, conversationManager)
    {
    }

    public async Task<CoreResponse> Invoke(string text, string sessionId)
    {
        return await base.Invoke(new CoreRequest
        {
            Prompt = text
        }, sessionId, "NWZQ7VJKHG", "D8OT4ASWHD");
    }

    protected override string AgentId => "907b65f0-6ce5-43cb-95d8-664b84edd33a";
}