using TalkToMe.Core.Interfaces;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Core.Services;

public class QueryCounterService: IQueryCounterService
{
    private const int QueriesLimit = 200;
    private readonly IQueryCounterRepository _repository;
    
    public QueryCounterService(IQueryCounterRepository repository)
    {
        _repository = repository;
    }

    public async Task CheckLimitOrThrowError(string userId)
    {
        var count = await _repository.IncrementCounterAsync(userId, 1);

        if (count >= QueriesLimit)
            throw new Exception("");
    }
}