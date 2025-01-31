using TalkToMe.Core.Builders;
using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents;

public class EnglishRetailerAgent : BaseAgent
{
    public EnglishRetailerAgent(IAIProviderFactory aiProviderFactory) :
        base(aiProviderFactory, AIProvider.AmazonBedrock, BedrockAIModelNames.Lama3_3_70b_v1)
    {
    }

    protected override string SystemPromt =>
        "I will provide you with an original text and my retelling of it. Your task is only to ask questions based on the given story and my retelling. Focus on asking clarifying questions about details that may be missing, unclear, or misinterpreted in my retelling, asking about important themes or ideas from the original text that should have been included, and asking questions to help me reflect more deeply on the text’s content and meaning. Only ask questions related to the original text and my retelling—no additional comments or analysis are needed.";

    public override async Task<CoreResponse> Invoke(string originalText, string retailing)
    {
        var systemPromt = await BuildSystemPromt();
        var promt = new StringBuilder(systemPromt);
        promt.Append($"This is original text: {originalText}. ");
        
        var request = new CoreRequestBuilder()
        .WithSystemInstruction(promt.ToString())
        .WithPrompt($"This is my retailing: {retailing}.")
        .Build();

        return await base.Invoke(request);
    }
}