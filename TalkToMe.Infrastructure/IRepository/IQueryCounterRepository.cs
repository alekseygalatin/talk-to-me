namespace TalkToMe.Infrastructure.IRepository;

public interface IQueryCounterRepository
{
    Task<int> IncrementCounterAsync(string userId, int incrementBy, int ttlDays);
}