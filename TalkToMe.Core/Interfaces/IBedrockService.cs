public interface IBedrockService
{
    Task<BedrockResponse> InvokeModelAsync(BedrockRequest request);
} 