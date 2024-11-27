using TalkToMe.Core.Enums;

namespace TalkToMe.Core.Interfaces
{
    public interface IAIProviderFactory
    {
        IAIProvider GetProvider(AIProvider provider);
    }
}
