using Amazon.DynamoDBv2.DataModel;
using TalkToMe.Domain.Constants;

namespace TalkToMe.Domain.Entities;

[DynamoDBTable(TableNames.WordTable)]
public class WordEntity
{
    [DynamoDBHashKey("userId")]
    public string UserId { get; set; } = default!;

    [DynamoDBRangeKey("word")]
    public string Word { get; set; } = default!;
    
    [DynamoDBProperty("translation")]
    public string Translation { get; set; }
    
    [DynamoDBProperty("example")]
    public string Example { get; set; }
    
    [DynamoDBProperty("includeIntoChat")]
    public bool IncludeIntoChat { get; set; }
}