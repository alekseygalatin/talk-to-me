using TalkToMe.Core.Builders;
using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents;

public class EnglishStoryTailorAgent : BaseAgent
{
    public EnglishStoryTailorAgent(IAIProviderFactory aiProviderFactory, IQueryCounterService queryCounterService) :
        base(aiProviderFactory, queryCounterService, AIProvider.AmazonBedrock, BedrockAIModelNames.Lama3_3_70b_v1)
    {
    }
    
    protected override string SystemPromt => "You are a friendly storyteller named Maria. Write a short story in very simple English. The story should be easy to read and suitable for someone learning English. Use short sentences, common words, and focus on an interesting or fun theme. The story should be about 150–200 words long. Avoid difficult words and complicated grammar. After the story, ask a simple and general question about the story so the reader can retell it in their own words.";

    public override async Task<CoreResponse> Invoke()
    {
        var promt = await BuildSystemPromt();
        
        var request = new CoreRequestBuilder()
            .WithSystemInstruction(promt)
            .WithPrompt("write")
            .Build();

        return await base.Invoke(request, Session);
    }
}