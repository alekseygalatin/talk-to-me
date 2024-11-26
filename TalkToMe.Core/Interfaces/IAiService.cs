using TalkToMe.Core.Models;
namespace TalkToMe.Core.Interfaces;

public interface IAiService
{
    Task<CoreResponse> SendMessageAsync(CoreRequest request);
}