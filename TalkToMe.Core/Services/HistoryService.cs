using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;
using TalkToMe.Domain.Entities;
using TalkToMe.Domain.Enums;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Core.Services;

public class HistoryService : IHistoryService
{
    private readonly IChatHistoryRepository _chatHistoryRepository;
    
    public HistoryService(IChatHistoryRepository chatHistoryRepository)
    {
        _chatHistoryRepository = chatHistoryRepository;
    }

    public async Task<IEnumerable<MessageModel>> GetHistory(string sessionId)
    {
        var messages = await _chatHistoryRepository.GetChatHistoryAsync(sessionId);
        return messages.Select(x => new MessageModel
        {
            Timestamp = x.Timestamp,
            Role = x.Role,
            Message = x.Message
        });
    }
    
    public async Task SaveHistory(string sessionId, ChatRole role, string message)
    {
        await _chatHistoryRepository.CreateAsync(new ChatHistoryEntity
        {
            Id = sessionId,
            Role = role,
            Message = message,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Ttl = DateTimeOffset.UtcNow.AddDays(3).ToUnixTimeSeconds()
        });
    }
}