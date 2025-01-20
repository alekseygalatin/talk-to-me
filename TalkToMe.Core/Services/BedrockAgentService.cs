using TalkToMe.Core.Models;
using Amazon.BedrockAgentRuntime;
using Amazon.BedrockAgentRuntime.Model;
using TalkToMe.Core.Interfaces;
using PayloadPart = Amazon.BedrockAgentRuntime.Model.PayloadPart;

namespace TalkToMe.Core.Services;

public class BedrockAgentService : IBedrockAgentService
{
    private readonly AmazonBedrockAgentRuntimeClient _client;

    public BedrockAgentService(IBedrockClientFactory clientFactory)
    {
        _client = clientFactory.CreateAgentClient();
    }
    
    public async Task<CoreResponse> Invoke(string input, string sessionId, string agentId, string agentAliasId)
    {
        var result = await _client.InvokeAgentAsync(new InvokeAgentRequest
        {
            AgentId = agentId,
            AgentAliasId = agentAliasId,
            SessionId = sessionId,
            MemoryId = sessionId,
            InputText = input,
        });
        
        if(result.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            var output = new MemoryStream();
            foreach(var eventStreamEvent in result.Completion)
            {
                var item = (PayloadPart)eventStreamEvent;
                {
                    await item.Bytes.CopyToAsync(output);
                }
            }

            var text = Encoding.UTF8.GetString(output.ToArray());
            return new CoreResponse
            {
                Response = text
            };
        }

        throw new Exception("Agent failed");
    }
}