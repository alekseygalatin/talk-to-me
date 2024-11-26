using TalkToMe.Core.Models;

namespace TalkToMe.Core.Interfaces;

public interface IAiModelService
{
    Task<CoreResponse> InvokeModelAsync(CoreRequest request);
}