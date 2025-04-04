using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using TalkToMe.Domain.Constants;
using TalkToMe.Domain.Entities;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Infrastructure.Repository
{
    public class FeedbackRepository : BaseRepository<Feedback>, IFeedbackRepository
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        public FeedbackRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }

        public async Task<long?> GetLastFeedbackDate(string userId)
        {
            var request = new QueryRequest
            {
                TableName = TableNames.FeedbacksTable,
                KeyConditionExpression = "UserId = :userId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":userId", new AttributeValue { S = userId } }
                },
                ScanIndexForward = false,
                Limit = 1
            };

            var response = await _dynamoDb.QueryAsync(request);

            var latestFeedbackTimeStampStr = response.Items.FirstOrDefault()?["CreatedAt"].N;

            if (long.TryParse(latestFeedbackTimeStampStr, out long latestFeedbackTimeStamp)) 
                return latestFeedbackTimeStamp;

            return null;
        }
    }
}
