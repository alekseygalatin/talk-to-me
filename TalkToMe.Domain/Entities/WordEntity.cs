using Amazon.DynamoDBv2.DataModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using TalkToMe.Domain.Constants;

namespace TalkToMe.Domain.Entities;

[DynamoDBTable(TableNames.WordsTable)]
public class WordEntity
{
    [DynamoDBHashKey]
    public string UserId { get; set; } = default!;

    [DynamoDBRangeKey]
    public string LanguageWord { get; set; } = default!;

    [DynamoDBIgnore]
    public string Language
    {
        get => LanguageWord.Split('#')[0]; 
    }

    [DynamoDBIgnore]
    public string Word
    {
        get => LanguageWord.Split('#')[1];
    }

    [DynamoDBProperty]
    public string Transcription { get; set; } = default!;

    [DynamoDBProperty]
    public string TranslationsJson { get; set; } = default!;

    [DynamoDBIgnore]
    public List<string> Translations
    {
        get => TranslationsJson != null
            ? JsonSerializer.Deserialize<List<string>>(TranslationsJson)
            : new List<string>();
        set => TranslationsJson = JsonSerializer.Serialize(value,
            new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
    }

    [DynamoDBProperty]
    public string Example { get; set; } = default!;

    [DynamoDBProperty]
    public bool IncludeIntoChat { get; set; }
}