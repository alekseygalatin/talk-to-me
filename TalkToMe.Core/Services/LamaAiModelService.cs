using TalkToMe.Core.Configuration;
using TalkToMe.Core.Exceptions;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Services;
//https://www.llama.com/docs/model-cards-and-prompt-formats/llama3_1/
public class LamaAiModelService : IAiModelService, IDisposable
{
    private readonly AmazonBedrockRuntimeClient _client;
    private readonly IConversationManager _conversationManager;
    private bool _disposed;
    private readonly string _modelId;

    public LamaAiModelService(
        IBedrockClientFactory clientFactory,
        IConversationManager conversationManager, 
        string modelId)
    {
        if (clientFactory == null) throw new ArgumentNullException(nameof(clientFactory));
        if (string.IsNullOrEmpty(modelId))
            throw new ArgumentNullException("modelId");

        _conversationManager = conversationManager;
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
            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine("<|begin_of_text|>");
            promptBuilder.AppendLine($"<|start_header_id|>system<|end_header_id|>{request.SystemInstruction}.");
            
            if (request.SupportHistory) {
                promptBuilder.AppendLine("The following section contains the conversation history for your reference. Do not include or repeat this history in your response. Only respond to the user's latest input after the conversation history:");
                var memories = await _conversationManager.GetMemories(request.Prompt, request.SessionId);
                foreach (var memory in memories)
                {
                    promptBuilder.AppendLine($"{memory.Role}: {memory.Message}");
                }
            }
            
            promptBuilder.AppendLine("<|eot_id|>");
            
            promptBuilder.AppendLine($"<|start_header_id|>user<|end_header_id|>{request.Prompt}.");
            promptBuilder.AppendLine("Your response must not include your role name. Provide only the content.<|eot_id|>");

            
            var requestBody = JsonSerializer.Serialize(new
            {
                prompt = promptBuilder.ToString(),
                max_gen_len = 512,
                temperature = 0.7,
                top_p = 0.9
            });

            var requestBytes = Encoding.UTF8.GetBytes(requestBody);
                
            var invokeRequest = new InvokeModelRequest
            {
                ModelId = _modelId,
                Body = new MemoryStream(requestBytes),
                ContentType = "application/json",
                Accept = "application/json"
            };

            var response = await _client.InvokeModelAsync(invokeRequest);
            
            using var reader = new StreamReader(response.Body);
            var responseBody = await reader.ReadToEndAsync();
            var parsedResponse = JsonSerializer.Deserialize<JsonDocument>(responseBody);
            var generationText = parsedResponse.RootElement
                .GetProperty("generation").GetString() ?? string.Empty;

            if (request.SupportHistory)
            {
                await _conversationManager.AddMemory($"{request.Prompt}. {responseBody}", new List<Dialog>
                {
                    new Dialog
                    {
                        Role = "user",
                        Message = request.Prompt
                    },
                    new Dialog
                    {
                        Role = "model",
                        Message = responseBody
                    }
                }, request.SessionId);
            }

            return new CoreResponse
            {
                Response = generationText,
                Metadata = new Dictionary<string, object>
                {
                    { "ModelId", _modelId },
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