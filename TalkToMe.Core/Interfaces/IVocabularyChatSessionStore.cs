using TalkToMe.Core.Models;

namespace TalkToMe.Core.Interfaces
{
    public interface IVocabularyChatSessionStore
    {
        Task<List<string>> CreateSession(string userId, string language, int count);
        VocabularyChatSession CurrentSession(string userId, string language);
        void RemoveSession(string userId, string language);

    }
}
