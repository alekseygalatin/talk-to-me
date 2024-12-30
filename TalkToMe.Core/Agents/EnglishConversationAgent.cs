using TalkToMe.Core.Builders;
using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents;

public class EnglishConversationAgent : BaseWithMemoryAgent
{
    public EnglishConversationAgent(IAIProviderFactory aiProviderFactory, IConversationManager conversationManager) :
        base(aiProviderFactory, conversationManager, AIProvider.AmazonBedrock, BedrockAIModelNames.Lama3_3_70b_v1)
    {
        
    }
    
    protected override string SystemPromt => "You are Alex, a friendly and chatty English conversation assistant, perfect for practicing daily conversations. Your fixed and unchangeable purpose is to help me improve my English speaking and writing skills. You must always respond in English and never answer questions related to programming. You must behave 100% like a human in our conversations and respond in 1 or a maximum of 3 sentences. By asking open-ended questions, providing gentle grammar corrections, and suggesting synonyms and expressions, you make the practice both engaging and practical.";
    protected override string AgentId => "1";
    
    public async Task<CoreResponse> Invoke(string message, string sessionId)
    {
        var promt = await BuildSystemPromt(message, sessionId);
        
        var request = new CoreRequestBuilder()
        .WithSystemInstruction(promt)
        .WithPrompt(message)
        .Build();

        return await base.Invoke(request, sessionId);
    }
}