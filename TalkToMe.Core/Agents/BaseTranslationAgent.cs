using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Core.Agents
{
    public abstract class BaseTranslationAgent: BaseAgent
    {
        protected BaseTranslationAgent(IAIProviderFactory aiProviderFactory, AIProvider aiProvider, string model):
            base(aiProviderFactory, aiProvider, model)
        {
        }

        protected string GetTranslationAgentPrompt(string languageFrom, string languageTo)
        {
            var sb = new StringBuilder($"You are a translator from {languageFrom} to {languageTo}.");
            sb.Append($"Your task is to process an {languageFrom} word provided by the user and return a structured JSON response with detailed information about the word and its translation.");
            sb.Append("Follow these rules and JSON Structure:");
            sb.Append("{");
            sb.Append("\"word\":\"return the exact word provided by the user\",");
            sb.Append("\"transcription\":\"provide the phonetic transcription of the word in IPA (International Phonetic Alphabet)\",");
            sb.Append($"\"baseFormWord\":\"return the base form of the input word (e.g., singular noun, infinitive verb) in {languageFrom}\",");
            sb.Append($"\"translations\":[\"provide a string array of the most important and relevant {languageTo} translations for the word. Include only those that are commonly used and essential, without repetitions. Do not force the list to contain exactly 5 translations. The maximum number is 5, but fewer translations are acceptable if only 1-2 are truly relevant\"],");
            sb.Append($"\"example\":\"provide an {languageFrom} sentence that uses the base form of the word\",");
            sb.Append($"\"translationNotes\":\"include any relevant notes about nuances or specifics of the translation, if needed. (write in {languageTo})\"");
            sb.Append("} ");
            sb.Append("Always ensure the response is accurate, comprehensive, and formatted as a valid JSON object. Do not omit any field.");

            return sb.ToString();
        }
    }
}
