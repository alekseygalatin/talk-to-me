using MongoDB.Bson;
using MongoDB.Driver;

namespace TalkToMe.Core.Services;

public class HistoryService
{
    private readonly IMongoCollection<Document> _mongoCollection;
    
    public HistoryService()
    {
        const string connectionUri = "mongodb+srv://galatinaleksei:fiiqZQXCnc2Zv0LF@talktome.zjjg2.mongodb.net/?retryWrites=true&w=majority&appName=TalkToMe";
        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        // Set the ServerApi field of the settings object to set the version of the Stable API on the client
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        // Create a new client and connect to the server
        var client = new MongoClient(settings);
        var database = client.GetDatabase("talktome");
        _mongoCollection = database.GetCollection<Document>("talktomecollection");
    }

    public async Task<IEnumerable<Document>> GetHistory(string sessionId)
    {
        var pipeline = new[]
        {
            new BsonDocument
            {
                { "$match", new BsonDocument { { "guideId", sessionId } } }
            },
            new BsonDocument
            {
                { "$sort", new BsonDocument { { "timeStamp", 1 } } } // Sort by timeStamp descending
            },
            new BsonDocument
            {
                { "$limit", 20 } // Limit to 10 latest messages
            }
        };

        var results = await _mongoCollection.Aggregate<Document>(pipeline).ToListAsync();

        return results.ToList();
    }
}