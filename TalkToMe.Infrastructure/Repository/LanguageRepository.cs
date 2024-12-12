using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using TalkToMe.Domain.Entities;
using TalkToMe.Infrastructure.IRepository;

namespace TalkToMe.Infrastructure.Repository
{
    public class LanguageRepository : BaseRepository<Language>, ILanguageRepository
    {
        public LanguageRepository(IAmazonDynamoDB dynamoDb): base(dynamoDb) 
        {
        }
        
        public async Task<List<Language>> GetAllLanguagesAsync(bool onlyActive = true)
        {
            var languages = await GetAllAsync();
            if (onlyActive)
                languages = languages.Where(x => x.Active).ToList();

            return languages;
        }
    }
}
