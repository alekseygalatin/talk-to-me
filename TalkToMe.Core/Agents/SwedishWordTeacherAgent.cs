using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Core.Agents;

public class SwedishWordTeacherAgent : BaseWordTeacherAgent
{
    public SwedishWordTeacherAgent(IAIProviderFactory aiProviderFactory,
        IQueryCounterService queryCounterService,
        IVocabularyChatSessionStore sessionStore)
       : base(aiProviderFactory, queryCounterService, sessionStore, AIProvider.AmazonBedrock, BedrockAIModelNames.Claude_3_5_Haiku)
    {
    }

    protected override string Language => "Swedish";
    protected override string LanguageCode => "sv-SE";
}