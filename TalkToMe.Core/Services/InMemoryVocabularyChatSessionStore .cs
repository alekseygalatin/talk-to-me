using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Services
{
    public class InMemoryVocabularyChatSessionStore : IVocabularyChatSessionStore
    {
        private readonly ConcurrentDictionary<string, VocabularyChatSession> _sessions = new();
        private readonly IServiceProvider _serviceProvider;

        public InMemoryVocabularyChatSessionStore(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<List<string>> CreateSession(string userId, string language, int count)
        {
            using var scope = _serviceProvider.CreateScope();
            var wordService = scope.ServiceProvider.GetRequiredService<IWordService>();
            var words = await wordService.GetRandomWords(userId, language, count);
            var wordsList = words.Select(x => x.Word).ToList();

            var session = new VocabularyChatSession(wordsList);
            var sessionKey = GetSessionKey(userId, language);

            _sessions.TryAdd(sessionKey, session);

            return wordsList;
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
