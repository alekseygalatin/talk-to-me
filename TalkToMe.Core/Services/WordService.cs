using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;
using TalkToMe.Core.Interfaces;
using TalkToMe.Domain.Entities;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Core.Services;

public class WordService : IWordService
{
    private readonly IBaseRepository<WordEntity> _repository;

    public WordService(IBaseRepository<WordEntity> repository)
    {
        _repository = repository;
    }

    public async Task<List<WordResponseDto>> GetWords(string userId)
    {
        var wordsList = await _repository.GetManyByIdAsync(userId);
        return wordsList.Select(w => new WordResponseDto
        {
            Word = w.Word,
            Example = w.Example,
            Translation = w.Translation,
            IncludeIntoChat = w.IncludeIntoChat
        }).ToList();
    }

    public async Task AddWordToDictionary(string userId, AddWordToDictionaryRequestDto dto)
    {
        await _repository.UpdateAsync(new WordEntity
        {
            UserId = userId,
            Word = dto.Word,
            Translation = dto.Translation,
            Example = dto.Example,
            IncludeIntoChat = dto.IncludeIntoChat
        });
    }
}