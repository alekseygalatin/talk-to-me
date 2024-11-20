using System.Runtime.InteropServices.JavaScript;
using Amazon.Runtime.Documents;
using TalkToMe.Core.Configuration;
using TalkToMe.Core.Exceptions;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Services;

public class LamaBedrockService : IBedrockService, IDisposable
{
    private readonly AmazonBedrockRuntimeClient _client;
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

    public async Task<CoreBedrockResponse> InvokeModelAsync(CoreBedrockRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
            
        var modelId = string.IsNullOrEmpty(request.ModelId) 
            ? _settings.DefaultModelId 
            : request.ModelId;

        try
        {
            var promptText = $"<<SYS>>{request.SystemInstruction}<<SYS>>{request.Prompt}";

            var converse = new ConverseRequest
            {
                ModelId = request.ModelId, // Replace with your mo
                Messages = new List<Message>
                {
                    // User provides the system instruction and input in the first message
                    new Message
                    {
                        Role = "user",
                        Content = new List<ContentBlock>
                        {
                            new ContentBlock
                            {
                                Text = promptText
                            }
                        }
                    }
                },
                AdditionalModelRequestFields = new Document
                {
                    {"max_gen_len", new Document(512)},
                    {"temperature", new Document(0.7)},
                    {"top_p", new Document(0.9)}
                }
            };

            var response = await _client.ConverseAsync(converse);

            return new CoreBedrockResponse
            {
                Response = response.Output.Message.Content[0].Text ?? string.Empty,
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