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

    public AwsAgentFactory(IAIProviderFactory aiProviderFactory, IHistoryService historyService, IWordService wordService, IBedrockAgentService bedrockAgentService)
    {
        _swedishTranslationAgent = new SwedishTranslationAgent(aiProviderFactory);
        _swedishStoryTailorAgent = new SwedishStoryTailorAgent(aiProviderFactory);
        _swedishConversationHelperAgent = new SwedishConversationHelperAgent(aiProviderFactory);
        _conversationSwedishAgent = new ConversationSwedishAgent(bedrockAgentService, historyService);
        _wordTeacherSwedishAgent = new WordTeacherSwedishAgent(bedrockAgentService, wordService, historyService);
        
        _englishTranslationAgent = new EnglishTranslationAgent(aiProviderFactory);
        _englishStoryTailorAgent = new EnglishStoryTailorAgent(aiProviderFactory);
        _englishConversationHelperAgent = new EnglishConversationHelperAgent(aiProviderFactory);
        _conversationAgent = new ConversationEnglishAgent(bedrockAgentService, historyService);
        _wordTeacherEnglishAgent = new WordTeacherEnglishAgent(bedrockAgentService, wordService, historyService);

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