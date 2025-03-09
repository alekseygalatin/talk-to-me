using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Core.Agents;

public class EnglishWordTeacherAgent : BaseWordTeacherAgent
{
    public EnglishWordTeacherAgent(IAIProviderFactory aiProviderFactory,
        IQueryCounterService queryCounterService,
        IVocabularyChatSessionStore sessionStore)
        : base(aiProviderFactory, queryCounterService, sessionStore, AIProvider.AmazonBedrock, BedrockAIModelNames.Lama3_3_70b_v1)
    {
    }

    protected override string Language
    {
        get
        {
            return "English";
        }
    }

    protected override string LanguageCode
    {
        get
        {
            return "en-US";
        }
    }
}
