namespace TalkToMe.Core.Models;

public class CoreResponse
{
    public string Response { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}