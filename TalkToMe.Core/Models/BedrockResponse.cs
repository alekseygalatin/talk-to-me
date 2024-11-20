public class BedrockResponse
{
    public string Response { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
} 