using TalkToMe.Core.Configuration;
using TalkToMe.Core.Exceptions;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Services;

public class TitanTextEmbedAiModelService : IAiModelService, IDisposable
{
    private readonly AmazonBedrockRuntimeClient _client;
    private readonly BedrockSettings _settings;
    private readonly IConversationManager _conversationManager;
    private bool _disposed;

    public TitanTextEmbedAiModelService(
        IBedrockClientFactory clientFactory, 
        BedrockSettings settings)
    {
        if (clientFactory == null) throw new ArgumentNullException(nameof(clientFactory));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _client = clientFactory.CreateClient();
    }

    public async Task<CoreBedrockResponse> InvokeModelAsync(CoreBedrockRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
            
        var modelId = string.IsNullOrEmpty(request.ModelId) 
            ? _settings.DefaultModelId 
            : request.ModelId;

        try
        {
            var requestBody = JsonSerializer.Serialize(new
            {
                inputText = request.Prompt
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
            var parsedResponse = JsonSerializer.Deserialize<JsonDocument>(responseBody);
            var generationText = parsedResponse.RootElement
                .GetProperty("embedding").ToString() ?? string.Empty;

            return new CoreBedrockResponse
            {
                Response = generationText,
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