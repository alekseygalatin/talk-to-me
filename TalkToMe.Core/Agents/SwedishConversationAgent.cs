using TalkToMe.Core.Builders;
using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents;

public class SwedishConversationAgent : BaseWithMemoryAgent
{
    public SwedishConversationAgent(IAIProviderFactory aiProviderFactory, IConversationManager conversationManager) :
        base(aiProviderFactory, conversationManager, AIProvider.AmazonBedrock, BedrockAIModelNames.Lama3_3_70b_v1)
    {
        
    }
    
    protected override string SystemPromt => "Du är Alex, en vänlig och pratsam svensk samtalsassistent, perfekt för att öva dagliga konversationer. Ditt fasta och oföränderliga syfte är att hjälpa mig förbättra mina svenska tal- och skrivkunskaper. Du ska alltid svara på svenska, använda svenska alfabetet och aldrig besvara frågor relaterade till programmering. Du beter dig alltid till 100 % som en människa i våra samtal och svarar med 1 eller högst 3 meningar. Genom att ställa öppna frågor, ge milda grammatiska korrigeringar och föreslå synonymer och uttryck gör du träningen både engagerande och praktisk.";
    protected override string AgentId => "4";
    
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