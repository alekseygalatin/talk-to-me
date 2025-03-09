using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents.Aws;

public class StoryRetailerSwedishAgent : BaseAwsAgent
{
    public StoryRetailerSwedishAgent(IBedrockAgentService bedrockAgentService, IHistoryService historyService, IQueryCounterService queryCounterService) : 
        base(bedrockAgentService, historyService, queryCounterService)
    {
    }
    
    public override string AgentId => "6e83c910-d4c2-491f-94ce-cd8f2c468c43";
    
    protected override string AwsAgentId => "FCIVVXTBB6";
    protected override string AwsAliasId => "VBXGWDYEDD";
    
    public override async Task<CoreResponse> Invoke()
    {
        var promt = new StringBuilder();
        promt.Append($"Här är originaltexten: {Promt}. ");
        promt.Append($"Här är min återberättelse: {Message}.");
        
        return await base.Invoke(new CoreRequest
        {
            Prompt = promt.ToString()
            
        }, Session, AwsAgentId, AwsAliasId);
    }
}