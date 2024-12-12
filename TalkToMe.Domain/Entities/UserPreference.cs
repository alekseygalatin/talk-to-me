using Amazon.DynamoDBv2.DataModel;
using TalkToMe.Domain.Constants;

namespace TalkToMe.Domain.Entities
{
    [DynamoDBTable(TableNames.UserPreferences)]
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

        [DynamoDBProperty]
        public string CurrentLanguageToLearn { get; set; } = default!;
    }
}
