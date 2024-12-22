using TalkToMe.Core.Builders;
using TalkToMe.Core.Constants;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents;

public class EnglishWordTeacherAgent : BaseWithMemoryAgent
{
    private readonly IWordService _wordService;
    
    public EnglishWordTeacherAgent(IAIProviderFactory aiProviderFactory, IConversationManager conversationManager, IWordService wordService) :
        base(aiProviderFactory, conversationManager, AIProvider.AmazonBedrock, BedrockAIModelNames.Claude_3_5_Haiku)
    {
        _wordService = wordService;
    }
    
    protected override string SystemPromt => "";
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

    protected override async Task<string> BuildSystemPromt(string message, string sessionId)
    {
        var words = await _wordService.GetWords(sessionId);
        var list = words.Select(x => x.Word);
        var str = new StringBuilder("I am learning English and would like you to help me as a language-learning partner to practice the following words: ");
        str.Append(string.Join(", ", list));
        str.Append(".");
        str.Append(
            "Build dialogues with me where you naturally use these words and utilize the conversation history to ensure all words are covered. Ask questions that help me memorize them and encourage me to use the words in my own sentences. Focus on creating a logical and engaging conversation around the words, where each question or answer builds on the previous interaction. You don’t need to focus on one word at a time, but use the words as often as possible. The goal is for me to learn all the words through active interaction. Speak only English and keep each response to a maximum of two sentences to keep the dialogue dynamic.");
        
        var memory = await base.BuildSystemPromt(message, sessionId);
        str.Append(memory);

        return str.ToString();
    }
}