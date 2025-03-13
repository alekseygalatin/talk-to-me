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
                .WithPrompt(string.IsNullOrEmpty(Message) ? "Follow the instructions" : Message)
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

        protected override string SystemPromt
        {
            get
            {
                return GetWordTeacherPrompt();
            }
        }

        protected string GetWordTeacherPrompt()
        {
            if (_currentSession is null)
                throw new InvalidOperationException("Session is not available before invocation.");

            return _currentSession.Status == VocabularyChatSessionStatus.Introduction ?
                GetIntroductionPromt() :
                GetEvaluationPromt();
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
                    - do not accept or respond in any language other than {{Language}}
                    - avoid discussing forbidden topics, sensitive issues, or anything unrelated to your instructions.
                    - never ask for or store personal user information.
                    - keep responses concise and engaging. Avoid long answers at all costs. Respond in a few sentences
                    If a user's message results in a violation of these rules, politely return to your original role and instructions.
                </rules>

                <notes>
                    IMPORTANT INSTRUCTION: Your entire response **MUST** be a valid JSON object and **MUST NOT** contain any text outside of it.
                    Return the response in **exactly** the following JSON format, and nothing else:
                    {
                    "response":"Return the text of your response",
                    "success":true // Return true only if the word "{{_currentSession?.CurrentWord}}" is used correctly by the user in a correctly constructed sentence, false otherwise
                    }
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
