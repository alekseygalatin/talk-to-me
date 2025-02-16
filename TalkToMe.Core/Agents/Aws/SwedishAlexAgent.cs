using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class SwedishAlexAgent : BaseAwsAgent
{
    public SwedishAlexAgent(IBedrockAgentService bedrockAgentService) : base(bedrockAgentService)
    {
    }

    public async Task<CoreResponse> Invoke(string text, string sessionId)
    {
        return await base.Invoke(new CoreRequest
        {
            Prompt = text
        }, sessionId, "NWZQ7VJKHG", "D8OT4ASWHD");
    }
}