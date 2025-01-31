using TalkToMe.Core.Agents;
using TalkToMe.Core.Agents.Aws;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Core.Factories;

public class AwsAgentFactory
{
    private SwedishTranslationAgent _swedishTranslationAgent;
    private SwedishStoryTailorAgent _swedishStoryTailorAgent;
    private SwedishConversationHelperAgent _swedishConversationHelperAgent;
    private SwedishAlexAgent _swedishAlexAgent;
    private SwedishEmmaAgent _swedishEmmaAgent;
    
    private EnglishTranslationAgent _englishTranslationAgent;
    private EnglishStoryTailorAgent _englishStoryTailorAgent;
    private EnglishConversationHelperAgent _englishConversationHelperAgent;
    private EnglishAlexAgent _englishAlexAgent;
    private EnglishEmmaAgent _englishEmmaAgent;

    private Dictionary<string, Dictionary<string, IAgent>> _agents;

    public AwsAgentFactory(IAIProviderFactory aiProviderFactory, IHistoryService historyService, IWordService wordService, IBedrockAgentService bedrockAgentService)
    {
        _swedishTranslationAgent = new SwedishTranslationAgent(aiProviderFactory);
        _swedishStoryTailorAgent = new SwedishStoryTailorAgent(aiProviderFactory);
        _swedishConversationHelperAgent = new SwedishConversationHelperAgent(aiProviderFactory);
        _swedishAlexAgent = new SwedishAlexAgent(bedrockAgentService, historyService);
        _swedishEmmaAgent = new SwedishEmmaAgent(bedrockAgentService, wordService, historyService);
        
        _englishTranslationAgent = new EnglishTranslationAgent(aiProviderFactory);
        _englishStoryTailorAgent = new EnglishStoryTailorAgent(aiProviderFactory);
        _englishConversationHelperAgent = new EnglishConversationHelperAgent(aiProviderFactory);
        _englishAlexAgent = new EnglishAlexAgent(bedrockAgentService, historyService);
        _englishEmmaAgent = new EnglishEmmaAgent(bedrockAgentService, wordService, historyService);

        _agents = new Dictionary<string, Dictionary<string, IAgent>>
        {
            {
                "alex", 
                new Dictionary<string, IAgent>
                {
                    {"sv-se", _swedishAlexAgent},
                    {"en-us", _englishAlexAgent}
                }
            },
            {
                "emma",
                new Dictionary<string, IAgent>
                {
                    {"sv-se", _swedishEmmaAgent},
                    {"en-us", _englishEmmaAgent}
                }
            },
            {
                "maria",
                new Dictionary<string, IAgent>
                {
                    {"sv-se", _swedishStoryTailorAgent},
                    {"en-us", _englishStoryTailorAgent}
                }
            },
            {
                "translation",
                new Dictionary<string, IAgent>
                {
                    {"sv-se", _swedishTranslationAgent},
                    {"en-us", _englishTranslationAgent}
                }
            },
            {
                "helper",
                new Dictionary<string, IAgent>
                {
                    {"sv-se", _swedishConversationHelperAgent},
                    {"en-us", _englishConversationHelperAgent}
                }
            }
        };
    }

    public IAgent GetAgent(string name, string locale)
    {
        return _agents[name][locale.ToLower()];
    }
}