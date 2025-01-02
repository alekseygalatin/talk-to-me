using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;

namespace TalkToMe.Core.Interfaces;

public interface IWordService
{
    Task<List<WordResponseDto>> GetWords(string userId, string langauge);
    Task AddWordToDictionary(string userId, AddWordToDictionaryRequestDto dto);
}