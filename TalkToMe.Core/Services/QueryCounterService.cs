using TalkToMe.Core.Exceptions;
using TalkToMe.Core.Interfaces;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Core.Services;

public class QueryCounterService: IQueryCounterService
{
    private const int QueriesLimit = 200;
    private const int IncrementBy = 1;
    private const int TtlDays = 3;
    private readonly IQueryCounterRepository _repository;
    
    public QueryCounterService(IQueryCounterRepository repository)
    {
        _repository = repository;
    }

    public async Task CheckLimitOrThrowError(string userId)
    {
        var count = await _repository.IncrementCounterAsync(userId, IncrementBy, TtlDays);

        if (count >= QueriesLimit)
            throw new QueryLimitExceeded();
    }
}