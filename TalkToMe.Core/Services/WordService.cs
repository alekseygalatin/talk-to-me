using AutoMapper;
using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;
using TalkToMe.Core.Interfaces;
using TalkToMe.Domain.Entities;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Core.Services;

public class WordService : IWordService
{
    private readonly IBaseRepository<WordEntity> _repository;
    private readonly IMapper _mapper;

    public WordService(IBaseRepository<WordEntity> repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<WordResponseDto>> GetWords(string userId)
    {
        var wordsList = await _repository.GetManyByIdAsync(userId);
        return _mapper.Map<List<WordResponseDto>>(wordsList);
    }

    public async Task AddWordToDictionary(string userId, AddWordToDictionaryRequestDto dto)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        if (dto is null)
            throw new ArgumentNullException(nameof(dto));

        var word = _mapper.Map<WordEntity>(dto);
        word.UserId = userId;
        word.IncludeIntoChat = true;

        await _repository.CreateAsync(word);
    }
}