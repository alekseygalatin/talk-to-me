using System.Runtime.InteropServices.JavaScript;
using Amazon.Runtime.Documents;
using TalkToMe.Core.Configuration;
using TalkToMe.Core.Exceptions;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Services;
//https://www.llama.com/docs/model-cards-and-prompt-formats/llama3_1/
public class LamaAiModelService : IAiModelService, IDisposable
{
    private readonly AmazonBedrockRuntimeClient _client;
    private readonly BedrockSettings _settings;
    private readonly IConversationManager _conversationManager;
    private bool _disposed;

    public LamaAiModelService(
        IBedrockClientFactory clientFactory, 
        BedrockSettings settings,
        IConversationManager conversationManager)
    {
        if (clientFactory == null) throw new ArgumentNullException(nameof(clientFactory));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _conversationManager = conversationManager;
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
            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine("<|begin_of_text|>");
            promptBuilder.AppendLine($"<|start_header_id|>system<|end_header_id|>{request.SystemInstruction}.");
            
            if (request.SupportHistory) {
                promptBuilder.AppendLine("The following section contains the conversation history for your reference. Do not include or repeat this history in your response. Only respond to the user's latest input after the conversation history:");
                _conversationManager.GetFormattedPrompt((role, content) =>
                {
                    promptBuilder.AppendLine($"{role}: {content}");
                });
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
                ModelId = modelId,
                Body = new MemoryStream(requestBytes),
                ContentType = "application/json",
                Accept = "application/json"
            };

            var response = await _client.InvokeModelAsync(invokeRequest);
            
            if (request.SupportHistory)
                _conversationManager.AddMessage("user", request.Prompt);
            
            using var reader = new StreamReader(response.Body);
            var responseBody = await reader.ReadToEndAsync();
            var parsedResponse = JsonSerializer.Deserialize<JsonDocument>(responseBody);
            var generationText = parsedResponse.RootElement
                .GetProperty("generation").GetString() ?? string.Empty;
            
            if (request.SupportHistory)
                _conversationManager.AddMessage("model", generationText);

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