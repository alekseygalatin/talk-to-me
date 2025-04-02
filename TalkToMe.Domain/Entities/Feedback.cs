using Amazon.DynamoDBv2.DataModel;
using TalkToMe.Domain.Constants;

namespace TalkToMe.Domain.Entities
{
    [DynamoDBTable(TableNames.FeedbacksTable)]
    public class Feedback
    {
        [DynamoDBHashKey] 
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [DynamoDBProperty]
        public string UserId { get; set; } = default!;

        [DynamoDBProperty]
        public string Content { get; set; } = default!;

        [DynamoDBProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DynamoDBGlobalSecondaryIndexHashKey("UserFeedbackIndex")]
        public string GSI_PK => UserId;
    }

}
