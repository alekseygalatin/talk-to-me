using TalkToMe.Core.Models;

namespace TalkToMe.Core.Interfaces;

public interface IAiModelService
{
    string ModelId { get; }
    Task<CoreResponse> SendMessageAsync(CoreRequest request);
}