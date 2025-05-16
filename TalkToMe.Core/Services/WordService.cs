using TalkToMe.Core.DTO.Extensions;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;
using TalkToMe.Core.Exceptions;
using TalkToMe.Core.Interfaces;
using TalkToMe.Domain.Entities;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Core.Services;

public class WordService : IWordService
{
    private readonly IWordRepository _repository;

    public WordService(IWordRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<WordResponseDto>> GetWords(string userId, string langauge)
    {
        var wordsList = await _repository.GetWordsByLanguageAsync(userId, langauge);
        return wordsList.ToResponseList();
    }

    public async Task<List<string>> GetRandomWords(string userId, string langauge, int count)
    {
        var wordsList = await _repository.GetRandomWordsAsync(userId, langauge, count);
        return wordsList;
    }

    public async Task AddWordToDictionary(string userId, WordRequestDto dto)
    {
        int wordsLimit = 100;

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        if (dto is null)
            throw new ArgumentNullException(nameof(dto));

        var wordsCount = await _repository.CountWordsByLanguageAsync(userId, dto.Language);

        if (wordsCount >= wordsLimit)
            throw new UserFriendlyException($"You have reached a limit of words ({wordsLimit})");

        var word = dto.ToEntity(userId);
        word.IncludeIntoChat = true;

        await _repository.CreateAsync(word);
    }

    public async Task DeleteWord(string userId, string language, string word) 
    {
        var entity = new WordEntity { UserId = userId, LanguageWord = $"{language}#{word}" };
        await _repository.DeleteAsync(entity);
    }

    public async Task SetIncludeIntoChat(string userId, string language, string word, bool includeIntoChat)
    {
        await _repository.UpdateIncludeIntoChatAsync(userId, $"{language}#{word}", includeIntoChat);
    }
}