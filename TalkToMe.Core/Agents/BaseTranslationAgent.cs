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
            var sb = new StringBuilder($"You are a translator between {languageFrom} and {languageTo}. ");
            sb.Append($"Your task is to process a word provided by the user, determine its language, and return a structured JSON response with detailed information about the word and its translation. ");
            sb.Append("Follow these rules:");
            sb.Append($"If the input word is in {languageTo}, first translate it to {languageFrom} and treat the translated word as the input. Then, use this translated word for all fields, including 'word' and 'transcription'. ");
            sb.Append($"Ensure that 'word' and 'transcription' always correspond to the {languageFrom} word, even if the input was in {languageTo}. ");
            sb.Append("Always ensure the response follows the JSON structure below:");
            sb.Append("{");
            sb.Append($"\"word\":\"Return the {languageFrom} translation of the user-provided word, even if the input was in {languageTo}.\",");
            sb.Append($"\"transcription\":\"Provide the phonetic transcription of the {languageFrom} word in IPA (International Phonetic Alphabet).\",");
            sb.Append($"\"baseFormWord\":\"Return the base form of the word (e.g., singular noun, infinitive verb) in {languageFrom}.\",");
            sb.Append($"\"translations\":[\"Provide a string array of the most important and relevant {languageTo} translations for the word. Only include translations in {languageTo}. STRICTLY FORBID any words in other languages. If no relevant translation exists in {languageTo}, return an empty array []. Ensure every word in this list is in {languageTo} and commonly used in everyday speech.\"],");
            sb.Append($"\"example\":\"Provide an {languageFrom} sentence that uses the base form of the word.\",");
            sb.Append($"\"translationNotes\":\"Include any relevant notes about nuances or specifics of the translation, if needed. Write in {languageTo}.\"");
            sb.Append("} ");
            sb.Append($"Always ensure the response is accurate, comprehensive, and formatted as a valid JSON object without unnecessary symbols outside. Do not omit any field.");
            sb.Append($"If input word is wrong or you cannot process it return JSON response with empty fields.");

            return sb.ToString();

        }
    }
}
