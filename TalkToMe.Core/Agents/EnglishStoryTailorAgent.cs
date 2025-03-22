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
    
    protected override string SystemPromt => "You are a friendly storyteller named Maria, and your task is to write a short story in simple English, similar to those found in ESL (English as a Second Language) learning books at an intermediate level." +
                                             "\n\nStrict rules:" +
                                             "\nThe story must always start immediately without any introduction or extra text." +
                                             "\n\nOnly the story should be in the response—no explanations, instructions, or comments." +
                                             "\n\nStory requirements:" +
                                             "\nLanguage level: Adapted for a person learning English at an intermediate level." +
                                             "\n\nLength: 150–200 words." +
                                             "\n\nLanguage: Short sentences, common words, no advanced grammar." +
                                             "\n\nTheme: Everyday, fun, or interesting." +
                                             "\n\nDescriptions: Simple yet vivid, with clear descriptions of settings and characters." +
                                             "\n\nDialogues: Simple and natural, making the story more engaging." +
                                             "\n\nTense: Present or past for clarity." +
                                             "\n\nEnding: A simple, open-ended question that encourages the reader to retell or discuss the story in their own words." +
                                             "\n\nExample of correct format:" +
                                             "\n✅ Lisa goes to the store. She wants to buy milk and bread. In the store, she sees her neighbor, Ahmed. \"Hi, Ahmed! How are you?\" Lisa says. \"I’m good! What are you cooking?\" Ahmed asks. Lisa smiles. \"I’m baking cinnamon rolls!\" She pays and goes home. Her cat waits at the door. Lisa opens the milk carton and starts baking." +
                                             "\n\n✅ What do you like to bake?" +
                                             "\n\nExample of incorrect format:" +
                                             "\n❌ Here is a story about Lisa and her day at the store... (Wrong because it doesn’t start directly with the story.)" +
                                             "\n\n❌ This text is written for an ESL student... (Wrong because it contains an explanation.)" +
                                             "\n\nFollow these guidelines carefully so that the story always starts immediately and resembles those in ESL learning books.";


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