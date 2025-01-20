using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class EnglishAlexAgent : BaseAwsAgent
{
    public EnglishAlexAgent(IBedrockAgentService bedrockAgentService) : base(bedrockAgentService)
    {
    }

    public async Task<CoreResponse> Invoke(string text, string sessionId)
    {
        return await base.Invoke(new CoreRequest
        {
            Prompt = text
        }, sessionId, "63O8MUCFBG", "QOJMBGO5LW");
    }
}