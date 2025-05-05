using Amazon.DynamoDBv2.DataModel;
using TalkToMe.Domain.Constants;

namespace TalkToMe.Domain.Entities
{
    [DynamoDBTable(TableNames.SubscriptionsTable)]
    public class Subscription
    {
        [DynamoDBHashKey]
        public string UserId { get; set; } = default!;

        [DynamoDBProperty]
        public string Comment { get; set; } = default!;

        [DynamoDBProperty]
        public DateTime CreatedAt { get; set; }
    }

}
