using TalkToMe.Core.Models;

namespace TalkToMe.Core.Interfaces;

public interface IAiService
{
    Task<CoreBedrockResponse> InvokeModelAsync(CoreBedrockRequest request);
}