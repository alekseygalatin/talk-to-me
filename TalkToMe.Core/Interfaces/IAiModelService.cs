using TalkToMe.Core.Models;

namespace TalkToMe.Core.Interfaces;

public interface IAiModelService
{
    Task<CoreBedrockResponse> InvokeModelAsync(CoreBedrockRequest request);
}