using Amazon.DynamoDBv2;
using TalkToMe.Domain.Entities;

namespace TalkToMe.Infrastructure.Repository;

public class WordRepository : BaseRepository<WordEntity>
{
    public WordRepository(IAmazonDynamoDB dynamoDb): base(dynamoDb) 
    {
    }
}