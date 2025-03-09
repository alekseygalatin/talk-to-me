namespace TalkToMe.Core.Interfaces;

public interface IQueryCounterService
{
    Task CheckLimitOrThrowError(string userId);
}