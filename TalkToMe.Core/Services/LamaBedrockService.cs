namespace TalkToMe.Core.Services;

public class LamaBedrockService : IBedrockService, IDisposable
{
    private readonly IAmazonBedrockRuntime _client;
    private readonly BedrockSettings _settings;
    private bool _disposed;

    public LamaBedrockService(
        IBedrockClientFactory clientFactory, 
        BedrockSettings settings)
    {
        if (clientFactory == null) throw new ArgumentNullException(nameof(clientFactory));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _client = clientFactory.CreateClient();
    }

    public async Task<BedrockResponse> InvokeModelAsync(BedrockRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
            
        var modelId = string.IsNullOrEmpty(request.ModelId) 
            ? _settings.DefaultModelId 
            : request.ModelId;

        try
        {
            // Format request based on the model
            var requestBody = JsonSerializer.Serialize(new
            {
                prompt = $"{request.Prompt}",
                max_gen_len = 512,
                temperature = 0.7,
                top_p = 0.9
            });

            var requestBytes = Encoding.UTF8.GetBytes(requestBody);
                
            var invokeRequest = new InvokeModelRequest
            {
                ModelId = modelId,
                Body = new MemoryStream(requestBytes),
                ContentType = "application/json",
                Accept = "application/json"
            };

            var response = await _client.InvokeModelAsync(invokeRequest);
                
            using var reader = new StreamReader(response.Body);
            var responseBody = await reader.ReadToEndAsync();

            // Parse the response based on model
            var parsedResponse = JsonSerializer.Deserialize<JsonDocument>(responseBody);
                
            var completionText = parsedResponse.RootElement
                .GetProperty("generation").GetString();

            return new BedrockResponse
            {
                Response = completionText ?? string.Empty,
                Metadata = new Dictionary<string, object>
                {
                    { "ModelId", modelId },
                    { "RequestId", response.ResponseMetadata.RequestId }
                }
            };
        }
        catch (Exception ex)
        {
            throw new BedrockServiceException("Failed to invoke Bedrock model", ex);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
            
        if (disposing)
        {
            _client?.Dispose();
        }

        _disposed = true;
    }
}