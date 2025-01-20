using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class EnglishAlexAgent : BaseAwsAgent
{
    public EnglishAlexAgent(IBedrockAgentService bedrockAgentService, IConversationManager conversationManager) : base(bedrockAgentService, conversationManager)
    {
    }

    public async Task<CoreResponse> Invoke(string text, string sessionId)
    {
        return await base.Invoke(new CoreRequest
        {
            Prompt = text
        }, sessionId, "63O8MUCFBG", "QOJMBGO5LW");
    }

    protected override string AgentId => "5a981447-3092-41d4-b6e2-771bcfce3252";
}