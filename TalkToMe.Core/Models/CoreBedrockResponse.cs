namespace TalkToMe.Core.Models;

public class CoreBedrockResponse
{
    public string Response { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}