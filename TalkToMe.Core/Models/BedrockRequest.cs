public class BedrockRequest
{
    public string Prompt { get; set; }
    public string ModelId { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
} 