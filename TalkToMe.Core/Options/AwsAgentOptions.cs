namespace TalkToMe.Core.Options;

public class AwsAgentOptions
{
    public string ConversationAgentSeId => Environment.GetEnvironmentVariable("BEDROCK_AGENT_ALEX_SE_ID");
    public string ConversationAgentAliasSeId => Environment.GetEnvironmentVariable("BEDROCK_AGENT_ALIAS_ALEX_SE_ID");
    
    public string StoryRetailerAgentSeId => Environment.GetEnvironmentVariable("BEDROCK_AGENT_MARIA_SE_ID");
    public string StoryRetailerAgentAliasSeId => Environment.GetEnvironmentVariable("BEDROCK_AGENT_ALIAS_MARIA_SE_ID");
    
    public string ConversationAgentEnId => Environment.GetEnvironmentVariable("BEDROCK_AGENT_ALEX_EN_ID");
    public string ConversationAgentAliasEnId => Environment.GetEnvironmentVariable("BEDROCK_AGENT_ALIAS_ALEX_EN_ID");
    
    public string StoryRetailerAgentEnId => Environment.GetEnvironmentVariable("BEDROCK_AGENT_MARIA_EN_ID");
    public string StoryRetailerAgentAliasEnId => Environment.GetEnvironmentVariable("BEDROCK_AGENT_ALIAS_MARIA_EN_ID");
}