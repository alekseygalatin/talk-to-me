using System.Collections.Concurrent;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Services
{
    public class InMemoryVocabularyChatSessionStore : IVocabularyChatSessionStore
    {
        private readonly ConcurrentDictionary<string, VocabularyChatSession> _sessions = new();
        private readonly IWordService _wordService;

        public InMemoryVocabularyChatSessionStore(IWordService wordService)
        {
            _wordService = wordService;
        }

        public async Task<List<string>> CreateSession(string userId, string language, int count)
        {
            var words = await _wordService.GetRandomWords(userId, language, count);
            var session = new VocabularyChatSession(words);
            var sessionKey = GetSessionKey(userId, language);

            _sessions.TryAdd(sessionKey, session);

            return words;
        }

        public VocabularyChatSession CurrentSession(string userId, string language)
        {
            var sessionKey = GetSessionKey(userId, language);
            if (_sessions.TryGetValue(sessionKey, out var session))
            {
                return session;
            }

            throw new KeyNotFoundException($"Session not found for user {userId} and language {language}");
        }

        public void RemoveSession(string userId, string language)
        {
            _sessions.TryRemove(GetSessionKey(userId, language), out _);
        }

        private static string GetSessionKey(string userId, string language) => $"{language}#{userId}";
    }

}
