using TalkToMe.Core.Configuration;
using TalkToMe.Core.Exceptions;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Services;

public class TitanTextEmbedAiModelService : IAiModelService, IDisposable
{
    private readonly AmazonBedrockRuntimeClient _client;
    private readonly IConversationManager _conversationManager;
    private bool _disposed;
    private readonly string _modelId;

    public TitanTextEmbedAiModelService(
        IBedrockClientFactory clientFactory,
        string modelId)
    {
        if (clientFactory == null) throw new ArgumentNullException(nameof(clientFactory));
        _client = clientFactory.CreateClient();
        _modelId = modelId; 
    }

    public string ModelId
    {
        get { return _modelId; }
    }

    public async Task<CoreResponse> SendMessageAsync(CoreRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        try
        {
            var requestBody = JsonSerializer.Serialize(new
            {
                inputText = request.Prompt
            });

            var requestBytes = Encoding.UTF8.GetBytes(requestBody);
                
            var invokeRequest = new InvokeModelRequest
            {
                ModelId = ModelId,
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

            return new CoreResponse
            {
                Response = generationText,
                Metadata = new Dictionary<string, object>
                {
                    { "ModelId", ModelId },
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