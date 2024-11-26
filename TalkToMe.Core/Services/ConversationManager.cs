using MongoDB.Bson;
using MongoDB.Driver;
using TalkToMe.Core.Configuration;
using TalkToMe.Core.Factories;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Services;

public class Document
{
    public ObjectId Id { get; set; }
    public string GuideId { get; set; }  // Unique identifier for the guide
    public List<float> Embedding { get; set; }
    public List<Dialog> Dialog { get; set; }
}

public class Dialog
{
    public string Role { get; set; }
    public string Message { get; set; }
}

public class ConversationManager : IConversationManager
{
    private readonly IMongoCollection<Document> _mongoCollection;
    private readonly TitanTextEmbedAiModelService _titan;
    public ConversationManager()
    {
        const string connectionUri = "mongodb+srv://galatinaleksei:fiiqZQXCnc2Zv0LF@talktome.zjjg2.mongodb.net/?retryWrites=true&w=majority&appName=TalkToMe";
        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        // Set the ServerApi field of the settings object to set the version of the Stable API on the client
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        // Create a new client and connect to the server
        var client = new MongoClient(settings);
        var database = client.GetDatabase("talktome");
        _mongoCollection = database.GetCollection<Document>("talktomecollection");

        var factory = new BedrockClientFactory(new BedrockSettings
        {
            DefaultModelId = "",
            Region = "us-east-1"
        });
        
        _titan = new TitanTextEmbedAiModelService(factory, new BedrockSettings
        {
            DefaultModelId = "",
            Region = "us-east-1"
        });
    }
    public async Task AddMessage(string content, List<Dialog> dialogs)
    {
        var response = await _titan.InvokeModelAsync(new CoreBedrockRequest
        {
            ModelId = "amazon.titan-embed-text-v1",
            Prompt = content
        });

        var vector = JsonSerializer.Deserialize<List<float>>(response.Response);
        
        var document = new Document
        {
            GuideId = "guide123",
            Embedding = vector,
            Dialog = dialogs
        };

        await _mongoCollection.InsertOneAsync(document);
    }

    public async Task<IEnumerable<string>> GetFormattedPrompt(string promt, Action<string, string> buildFormatedPromt)
    {
        var response = await _titan.InvokeModelAsync(new CoreBedrockRequest
        {
            ModelId = "amazon.titan-embed-text-v1",
            Prompt = promt
        });

        var vector = JsonSerializer.Deserialize<List<float>>(response.Response);
        
        var searchPipeline = new BsonDocument
        {
            {
                "$vectorSearch", new BsonDocument
                {
                    { "index", "vector_index" }, // Replace with your actual index name
                    { "path", "Embedding" }, // Field where embeddings are stored
                    { "queryVector", new BsonArray(vector) }, // The embedding vector for search
                    { "numCandidates", 5 }, // Number of candidates for the search
                    { "limit", 5 } // Limit the number of returned results
                }
            }
        };

        var results = await _mongoCollection.Aggregate<Document>(new[] { searchPipeline,     new BsonDocument
        {
            { "$match", new BsonDocument { { "GuideId", "guide123" } } }
        }}).ToListAsync();

        foreach (var result in results)
        {
            foreach (var dialog in result.Dialog)
            {
                buildFormatedPromt(dialog.Role, dialog.Message);
            }
        }
        return new List<string>();
    }
}