using Amazon.DynamoDBv2.DataModel;
using TalkToMe.Domain.Constants;

namespace TalkToMe.Domain.Entities
{
    [DynamoDBTable(TableNames.Languages)]
    public class Language
    {
        [DynamoDBHashKey]
        public string Code { get; set; } = default!;

        [DynamoDBProperty]
        public string Name { get; set; } = default!;

        [DynamoDBProperty]
        public string EnglishName { get; set; } = default!;

        [DynamoDBProperty]
        public bool Active { get; set; }

        [DynamoDBProperty]
        public List<string> Pronouns { get; set; } = new List<string>();
    }
}
