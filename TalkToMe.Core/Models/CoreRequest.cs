namespace TalkToMe.Core.Models;

public class CoreRequest
{
    public string Prompt { get; set; }
    public string ModelId { get; set; }
    public string SystemInstruction { get; set; }
    public bool SupportHistory { get; set; }
}