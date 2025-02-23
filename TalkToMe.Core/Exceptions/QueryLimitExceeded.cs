namespace TalkToMe.Core.Exceptions;

public class QueryLimitExceeded : Exception
{
    public QueryLimitExceeded() 
        : base("Query limit exceeded.")
    {
    }
}