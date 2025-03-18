using TalkToMe.Core.DTO.Request;
using TalkToMe.Core.DTO.Response;

namespace TalkToMe.Core.Interfaces;

public interface IWordService
{
    Task<List<WordResponseDto>> GetWords(string userId, string langauge);
    Task<List<string>> GetRandomWords(string userId, string langauge, int count);
    Task AddWordToDictionary(string userId, WordRequestDto dto);
    Task DeleteWord(string userId, string language, string word);
    Task SetIncludeIntoChat(string userId, string language, string word, bool includeIntoChat);
}