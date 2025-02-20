using TalkToMe.Core.Agents;
using TalkToMe.Core.Agents.Aws;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Core.Factories;

public class AwsAgentFactory
{
    private SwedishTranslationAgent _swedishTranslationAgent;
    private SwedishStoryTailorAgent _swedishStoryTailorAgent;
    private SwedishConversationHelperAgent _swedishConversationHelperAgent;
    private ConversationSwedishAgent _conversationSwedishAgent;
    private WordTeacherSwedishAgent _wordTeacherSwedishAgent;
    
    private EnglishTranslationAgent _englishTranslationAgent;
    private EnglishStoryTailorAgent _englishStoryTailorAgent;
    private EnglishConversationHelperAgent _englishConversationHelperAgent;
    private ConversationEnglishAgent _conversationAgent;
    private WordTeacherEnglishAgent _wordTeacherEnglishAgent;

    private Dictionary<string, Dictionary<string, IAgent>> _agents;

    private string swedishLocale = "sv-se";
    private string englishLocale = "en-us";

    public AwsAgentFactory(IAIProviderFactory aiProviderFactory, IHistoryService historyService, IWordService wordService, IBedrockAgentService bedrockAgentService, IQueryCounterService queryCounterService)
    {
        _swedishTranslationAgent = new SwedishTranslationAgent(aiProviderFactory, queryCounterService);
        _swedishStoryTailorAgent = new SwedishStoryTailorAgent(aiProviderFactory, queryCounterService);
        _swedishConversationHelperAgent = new SwedishConversationHelperAgent(aiProviderFactory, queryCounterService);
        _conversationSwedishAgent = new ConversationSwedishAgent(bedrockAgentService, historyService, queryCounterService);
        _wordTeacherSwedishAgent = new WordTeacherSwedishAgent(bedrockAgentService, wordService, historyService, queryCounterService);
        
        _englishTranslationAgent = new EnglishTranslationAgent(aiProviderFactory, queryCounterService);
        _englishStoryTailorAgent = new EnglishStoryTailorAgent(aiProviderFactory, queryCounterService);
        _englishConversationHelperAgent = new EnglishConversationHelperAgent(aiProviderFactory, queryCounterService);
        _conversationAgent = new ConversationEnglishAgent(bedrockAgentService, historyService, queryCounterService);
        _wordTeacherEnglishAgent = new WordTeacherEnglishAgent(bedrockAgentService, wordService, historyService, queryCounterService);

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
                    {swedishLocale, _wordTeacherSwedishAgent},
                    {englishLocale, _wordTeacherEnglishAgent}
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