using Amazon.DynamoDBv2.DataModel;

namespace TalkToMe.Domain.Entities
{
    [DynamoDBTable("UserPreferences")]
    public class UserPreference
    {
        [DynamoDBHashKey]
        public string UserId { get; set; } = default!;

        [DynamoDBProperty]
        public string Name { get; set; } = default!;

        [DynamoDBProperty]
        public string Sex { get; set; } = default!;

        [DynamoDBProperty]
        public string NativeLanguage { get; set; } = default!;
    }
}
