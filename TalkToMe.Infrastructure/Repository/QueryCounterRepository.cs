using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using TalkToMe.Domain.Constants;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Infrastructure.Repository;

public class QueryCounterRepository: IQueryCounterRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;

    public QueryCounterRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }
    
    public async Task<int> IncrementCounterAsync(string userId, int incrementBy, int ttlDays)
    {
        var request = new UpdateItemRequest
        {
            TableName = TableNames.QueryCountersTable,
            Key = new Dictionary<string, AttributeValue>
            {
                { "UserId", new AttributeValue { S = userId } }
            },
            UpdateExpression = "SET CounterValue = if_not_exists(CounterValue, :zero) + :incr, #ttl = if_not_exists(#ttl, :ttl)",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#ttl", "Ttl" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":incr", new AttributeValue { N = incrementBy.ToString() } },
                { ":zero", new AttributeValue { N = "0" } },
                { ":ttl", new AttributeValue { N = DateTimeOffset.UtcNow.AddDays(ttlDays).ToUnixTimeSeconds().ToString() } }
            },
            ReturnValues = "UPDATED_NEW"
        };

        var response = await _dynamoDb.UpdateItemAsync(request);
        return int.Parse(response.Attributes["CounterValue"].N);
    }
}