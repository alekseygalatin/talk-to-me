using TalkToMe.Core.Exceptions;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Services;

public class NovaProModelService : IAiModelService, IDisposable
{
    private readonly AmazonBedrockRuntimeClient _client;
    private bool _disposed;
    private readonly string _modelId;

    public NovaProModelService(
        IBedrockClientFactory clientFactory,
        string modelId)
    {
        if (clientFactory == null) throw new ArgumentNullException(nameof(clientFactory));
        if (string.IsNullOrEmpty(modelId))
            throw new ArgumentNullException("modelId");
        
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
            var response = await _client.ConverseAsync(new ConverseRequest
            {
                ModelId = _modelId,
                System = new List<SystemContentBlock>
                {
                    new SystemContentBlock
                    {
                        Text = request.SystemInstruction
                    }
                },
                Messages = new List<Message>
                {
                    new Message
                    {
                        Role = ConversationRole.User,
                        Content = new List<ContentBlock>
                        {
                            new ContentBlock
                            {
                                Text = request.Prompt
                            }
                        }
                    }
                }
            });

            var generationText = response.Output.Message?.Content[0]?.Text;
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