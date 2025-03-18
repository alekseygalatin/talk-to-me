using TalkToMe.Core.Agents;
using TalkToMe.Core.Agents.Aws;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Options;

namespace TalkToMe.Core.Factories;

public class AwsAgentFactory
{
    private SwedishTranslationAgent _swedishTranslationAgent;
    private SwedishStoryTailorAgent _swedishStoryTailorAgent;
    private SwedishConversationHelperAgent _swedishConversationHelperAgent;
    private ConversationSwedishAgent _conversationSwedishAgent;
    private WordTeacherSwedishAgent _wordTeacherSwedishAgent;
    private SwedishWordTeacherAgent _swedishWordTeacherAgent;
    private StoryRetailerSwedishAgent _retailerSwedish;
    
    private EnglishTranslationAgent _englishTranslationAgent;
    private EnglishStoryTailorAgent _englishStoryTailorAgent;
    private EnglishConversationHelperAgent _englishConversationHelperAgent;
    private ConversationEnglishAgent _conversationAgent;
    private WordTeacherEnglishAgent _wordTeacherEnglishAgent;
    private EnglishWordTeacherAgent _englishWordTeacherAgent;
    private StoryRetailerEnglishAgent _retailerEnglish;

    private Dictionary<string, Dictionary<string, IAgent>> _agents;

    private string swedishLocale = "sv-se";
    private string englishLocale = "en-us";

    public AwsAgentFactory(
        IAIProviderFactory aiProviderFactory, 
        IHistoryService historyService, 
        IWordService wordService, 
        IBedrockAgentService bedrockAgentService, 
        IQueryCounterService queryCounterService,
        AwsAgentOptions awsAgentOptions,
        IVocabularyChatSessionStore _vocabularyChatSessionStore)
    {
        _swedishTranslationAgent = new SwedishTranslationAgent(aiProviderFactory, queryCounterService);
        _swedishStoryTailorAgent = new SwedishStoryTailorAgent(aiProviderFactory, queryCounterService);
        _swedishConversationHelperAgent = new SwedishConversationHelperAgent(aiProviderFactory, queryCounterService);
        _conversationSwedishAgent = new ConversationSwedishAgent(bedrockAgentService, historyService, queryCounterService, awsAgentOptions);
        _wordTeacherSwedishAgent = new WordTeacherSwedishAgent(bedrockAgentService, wordService, historyService, queryCounterService, awsAgentOptions);
        _swedishWordTeacherAgent = new SwedishWordTeacherAgent(aiProviderFactory, queryCounterService, _vocabularyChatSessionStore); /**/
        _retailerSwedish = new StoryRetailerSwedishAgent(bedrockAgentService, historyService, queryCounterService, awsAgentOptions);
        
        _englishTranslationAgent = new EnglishTranslationAgent(aiProviderFactory, queryCounterService);
        _englishStoryTailorAgent = new EnglishStoryTailorAgent(aiProviderFactory, queryCounterService);
        _englishConversationHelperAgent = new EnglishConversationHelperAgent(aiProviderFactory, queryCounterService);
        _conversationAgent = new ConversationEnglishAgent(bedrockAgentService, historyService, queryCounterService);
        _wordTeacherEnglishAgent = new WordTeacherEnglishAgent(bedrockAgentService, wordService, historyService, queryCounterService);
        _englishWordTeacherAgent = new EnglishWordTeacherAgent(aiProviderFactory, queryCounterService, _vocabularyChatSessionStore); /**/
        _retailerEnglish = new StoryRetailerEnglishAgent(bedrockAgentService, historyService, queryCounterService);

        _agents = new Dictionary<string, Dictionary<string, IAgent>>
        {
            {
                "alex", 
                new Dictionary<string, IAgent>
                {
                    {swedishLocale, _conversationSwedishAgent},
                    {englishLocale, _conversationAgent}
                }
            },
            {
                "emma",
                new Dictionary<string, IAgent>
                {
                    {swedishLocale, _swedishWordTeacherAgent},
                    {englishLocale, _englishWordTeacherAgent}
                }
            },
            {
                "maria",
                new Dictionary<string, IAgent>
                {
                    {swedishLocale, _swedishStoryTailorAgent},
                    {englishLocale, _englishStoryTailorAgent}
                }
            },
            {
                "maria-chat",
                new Dictionary<string, IAgent>
                {
                    {swedishLocale, _retailerSwedish},
                    {englishLocale, _retailerEnglish}
                }
            },
            {
                "translation",
                new Dictionary<string, IAgent>
                {
                    {swedishLocale, _swedishTranslationAgent},
                    {englishLocale, _englishTranslationAgent}
                }
            },
            {
                "helper",
                new Dictionary<string, IAgent>
                {
                    {swedishLocale, _swedishConversationHelperAgent},
                    {englishLocale, _englishConversationHelperAgent}
                }
            }
        };
    }

    public IAgent GetAgent(string name, string locale)
    {
        return _agents[name][locale.ToLower()];
    }
}