using TalkToMe.Core.Models;

namespace TalkToMe.Core.Interfaces;

public interface IBedrockService
{
    Task<CoreBedrockResponse> InvokeModelAsync(CoreBedrockRequest request);
}