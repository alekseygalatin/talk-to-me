namespace TalkToMe.Core.Options;

public class AwsAgentOptions
{
    public string ConversationAgentSeId => Environment.GetEnvironmentVariable("BEDROCK_AGENT_ALEX_SE_ID") ?? "JKQCFGDXVP";
    public string ConversationAgentAliasSeId => Environment.GetEnvironmentVariable("BEDROCK_AGENT_ALIAS_ALEX_SE_ID") ?? "939TLTLVWA";
    
    public string StoryRetailerAgentSeId => Environment.GetEnvironmentVariable("BEDROCK_AGENT_MARIA_SE_ID") ?? "YIFCXNQFDP";
    public string StoryRetailerAgentAliasSeId => Environment.GetEnvironmentVariable("BEDROCK_AGENT_ALIAS_MARIA_SE_ID") ?? "1HGFYIRHAW";
    
    public string WordTeacherAgentSeId => Environment.GetEnvironmentVariable("BEDROCK_AGENT_EMMA_SE_ID") ?? "SKIQSLYKVY";
    public string WordTeacherAgentAliasSeId => Environment.GetEnvironmentVariable("BEDROCK_AGENT_ALIAS_EMMA_SE_ID") ?? "QIPKX9YT5A";
}