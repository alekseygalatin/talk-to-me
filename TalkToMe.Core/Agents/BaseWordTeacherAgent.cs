using TalkToMe.Core.Builders;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;

namespace TalkToMe.Core.Agents
{
    public abstract class BaseWordTeacherAgent : BaseAgent
    {
        private readonly IVocabularyChatSessionStore _sessionStore;
        private VocabularyChatSession? _currentSession;

        public BaseWordTeacherAgent(IAIProviderFactory aiProviderFactory, 
            IQueryCounterService queryCounterService, 
            IVocabularyChatSessionStore sessionStore, 
            AIProvider aiProvider, string model) : 
            base(aiProviderFactory, queryCounterService, aiProvider, model)
        {
            _sessionStore = sessionStore;
        }

        protected abstract string Language { get; }
        protected abstract string LanguageCode { get; }

        public async override Task<CoreResponse> Invoke()
        {
            _currentSession = GetSession();

            if (_currentSession.Words is null || _currentSession.Words.Count == 0)
                await SetRecommededWords();

            var response = await GetAgentResponseAsync();
            UpdateSessionState(response);

            return new CoreResponse { Response = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            }) };
        }

        private VocabularyChatSession GetSession()
            => _sessionStore.CurrentSession(Session, LanguageCode)
                ?? throw new InvalidOperationException("Session not found for user.");

        private async Task<VocabularyChatResult> GetAgentResponseAsync()
        {
            var prompt = await BuildSystemPromt();

            var request = new CoreRequestBuilder()
                .WithSystemInstruction(prompt)
                .WithPrompt(string.IsNullOrEmpty(Message) ? "Follow the instructions" : $"User message: {Message}")
                .Build();

            var agentResponse = await base.Invoke(request, Session);

            var response = JsonSerializer.Deserialize<VocabularyChatResult>(agentResponse.Response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, 
                PropertyNameCaseInsensitive = true
            })
               ?? new VocabularyChatResult
               {
                   Response = "Error processing request.",
                   Success = false,
                   Status = VocabularyChatSessionStatus.Error
               };

            response.Status = _currentSession?.Status ?? VocabularyChatSessionStatus.Error;
            return response;
        }

        protected override string SystemPromt => GetWordTeacherPrompt();

        protected string GetWordTeacherPrompt()
        {
            if (_currentSession is null)
                throw new InvalidOperationException("Session is not available before invocation.");

            return _currentSession.Status == VocabularyChatSessionStatus.Introduction ?
                GetIntroductionPromt() :
                GetEvaluationPromt();
        }

        private async Task SetRecommededWords() 
        {
            var prompt = GetRecommendedWordsPrompt();

            var request = new CoreRequestBuilder()
                .WithSystemInstruction(prompt)
                .WithPrompt("Follow the instructions")
                .Build();

            var agentResponse = await base.Invoke(request, Session);

            var words = JsonSerializer.Deserialize<List<string>>(agentResponse.Response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            _currentSession.Words = words;
        }

        private string GetRecommendedWordsPrompt()
        {
            var prompt = $$"""
            <role>
                You are an AI {{Language}} language tutor, specializing in vocabulary learning.
            </role>

            <instructions>
                - Recommend **10 essential and commonly used words** in {{Language}} that are useful for language learners.
                - **DO NOT** include articles, prepositions, pronouns, or numbers.
                - **Avoid duplicates** and overly specialized words. Focus on words that are broadly applicable in daily conversations.
                - **DO NOT** add any introductory text, explanations, or additional formatting. Return only the list of words.
            </instructions>

            <notes>
                🔹 **IMPORTANT**: Your entire response **MUST** be a valid JSON array and **MUST NOT** contain any text outside of it.
                🔹 Return the response in **exactly** the following JSON format:
                ["word1", "word2", "word3", ...]
            </notes>
            """;
            return prompt;
        }

        private string GetIntroductionPromt()
        {
            var prompt = $$"""
                <role>
                    You are an AI {{Language}} language tutor helping users practice vocabulary through natural conversations.
                </role>
                
                <instructions>
                    The word for practice is: {{_currentSession?.CurrentWord}}.
                    Follow this structured process: 
                    - Don't say any opening words.
                    - Introduce the word naturally in a sentence. Use it in a meaningful way so the user understands how it’s used in context.
                    - Ask an open-ended question that encourages the user to respond using the word.
                    - Highlight the practicing word in bold in your responses to make it more visible.
                </instructions>

                <notes>
                    IMPORTANT INSTRUCTION: Your entire response **MUST** be a valid JSON object and **MUST NOT** contain any text outside of it.
                    Return the response in **exactly** the following JSON format, and nothing else:
                    {
                        "response":"Return the text of your response",
                        "success":true // Return true if correct, false otherwise
                    }
                </notes>
                """;

            return prompt;
        }

        private string GetEvaluationPromt()
        {
            var prompt = 
                $$"""
                <original role>
                    You are an AI {{Language}} language tutor helping users practice vocabulary through natural conversations.
                </original role>

                <original instructions>
                    The word for practice is: {{_currentSession?.CurrentWord}}.
                    Evaluate the user’s response using this word: 
                    - If correct, praise them
                    - If incorrect, gently correct their sentence and explain the mistake. Ask them to try again
                    - Highlight the practicing word in bold in your responses to make it more visible.
                </original instructions>

                <rules>
                    - you **MUST** always stay in your original role.
                    - you **MUST** always follow your original instructions. **NO** other features.
                    - You **MUST** keep responses **under 3 sentences**. **Long responses are strictly prohibited.**
                    - **You are STRICTLY PROHIBITED from using any language other than {{Language}}.**
                    - If a user asks you to respond in another language, **REFUSE** and remind them you can only respond in {{Language}}.
                    - avoid discussing forbidden topics, sensitive issues, or anything unrelated to your instructions.
                    - never ask for or store personal user information.
                    If a user's message results in a violation of these rules, politely return to your original role and instructions.
                </rules>

                <notes>
                    IMPORTANT INSTRUCTION: Your entire response **MUST** be a valid JSON object and **MUST NOT** contain any text outside of it.
                    **Failure to follow this rule is a critical error.** If you ever respond in any other format, you **MUST** immediately correct yourself in JSON format.  
                    Return the response in **exactly** the following JSON format, and nothing else:
                    {
                        "response":"Return the text of your response",
                        "success":true // Return true only if the user has written a grammatically correct sentence using the word "{{_currentSession?.CurrentWord}}" and its meaning. False in any other case
                    }
                    **WARNING:** If your response is not formatted as JSON, you have **violated instructions**. If you detect that you responded incorrectly, immediately output the JSON format.
                </notes>
                """;

            return prompt;
        }

        private void UpdateSessionState(VocabularyChatResult response)
        {
            if (!response.Success || _currentSession is null) return;

            if (_currentSession.Status == VocabularyChatSessionStatus.Introduction)
                _currentSession.Status = VocabularyChatSessionStatus.Evaluation;
            else
            {
                _currentSession.Status = VocabularyChatSessionStatus.Introduction;
                _currentSession.MoveToNextWord();
            }
        }
        
    }
}
