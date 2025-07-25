﻿using TalkToMe.Domain.Entities;

namespace TalkToMe.Infrastructure.IRepository
{
    public interface IWordRepository : IBaseRepository<WordEntity>
    {
        Task<int> CountWordsByLanguageAsync(string userId, string language);
        Task<List<WordEntity>> GetWordsByLanguageAsync(string userId, string language);
        Task<List<string>> GetRandomWordsAsync(string userId, string language, int count);
        Task<WordEntity?> GetWordAsync(string userId, string language, string word);
        Task DeleteAsync(WordEntity word);
        Task UpdateIncludeIntoChatAsync(string userId, string languageWord, bool includeIntoChat);
    }
}
