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

        public async Task<DateTime?> GetLastFeedbackDate(string userId)
        {
            var request = new QueryRequest
            {
                TableName = TableNames.FeedbacksTable,
                IndexName = "UserFeedbackIndex", 
                KeyConditionExpression = "GSI_PK = :userId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":userId", new AttributeValue { S = userId } }
                },
                ProjectionExpression = "CreatedAt", 
                ScanIndexForward = false,
                Limit = 1 
            };

            var response = await _dynamoDb.QueryAsync(request);
            var latestFeedbackDateStr = response.Items.FirstOrDefault()?["CreatedAt"].S;

            if (DateTime.TryParse(latestFeedbackDateStr, out DateTime latestFeedbackDate)) 
            {
                return latestFeedbackDate;
            }

            return null;
        }
    }
}
