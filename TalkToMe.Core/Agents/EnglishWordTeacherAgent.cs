using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Core.Agents;

public class EnglishWordTeacherAgent : BaseWordTeacherAgent
{
    public EnglishWordTeacherAgent(IAIProviderFactory aiProviderFactory,
        IQueryCounterService queryCounterService,
        IVocabularyChatSessionStore sessionStore)
        : base(aiProviderFactory, queryCounterService, sessionStore, AIProvider.AmazonBedrock, BedrockAIModelNames.Claude_3_5_Haiku)
    {
    }

    protected override string Language => "English";
    protected override string LanguageCode => "en-US";
    
}
