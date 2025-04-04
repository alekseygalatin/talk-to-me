using Amazon.DynamoDBv2.DataModel;
using TalkToMe.Domain.Constants;

namespace TalkToMe.Domain.Entities
{
    [DynamoDBTable(TableNames.FeedbacksTable)]
    public class Feedback
    {
        [DynamoDBHashKey]
        public string UserId { get; set; } = default!;

        [DynamoDBProperty]
        public string Content { get; set; } = default!;

        [DynamoDBRangeKey]
        public long CreatedAt { get; set; }
    }

}
