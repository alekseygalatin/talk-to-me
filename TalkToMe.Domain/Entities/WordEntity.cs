using Amazon.DynamoDBv2.DataModel;
using TalkToMe.Domain.Constants;

namespace TalkToMe.Domain.Entities;

[DynamoDBTable(TableNames.WordsTable)]
public class WordEntity
{
    [DynamoDBHashKey]
    public string UserId { get; set; } = default!;

    [DynamoDBRangeKey]
    public string Langauge { get; set; } = default!;

    [DynamoDBRangeKey]
    public string Word { get; set; } = default!;

    [DynamoDBProperty]
    public string Transcription { get; set; } = default!;

    [DynamoDBProperty]
    public string BaseFormWord { get; set; } = default!;

    [DynamoDBProperty]
    public List<string> Translations { get; set; } = new List<string>();

    [DynamoDBProperty]
    public string Example { get; set; } = default!;

    [DynamoDBProperty]
    public bool IncludeIntoChat { get; set; }
}