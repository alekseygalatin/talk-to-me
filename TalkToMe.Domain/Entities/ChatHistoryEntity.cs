using Amazon.DynamoDBv2.DataModel;
using TalkToMe.Domain.Constants;
using TalkToMe.Domain.Enums;

namespace TalkToMe.Domain.Entities;

[DynamoDBTable(TableNames.ChatHistory)]
public class ChatHistoryEntity
{
    [DynamoDBHashKey]
    public string Id { get; set; }

    [DynamoDBRangeKey]
    public long Timestamp { get; set; }
    
    [DynamoDBProperty]
    public ChatRole Role { get; set; }
    
    [DynamoDBProperty]
    public string Message { get; set; }

    [DynamoDBProperty]
    public long Ttl { get; set; }
}