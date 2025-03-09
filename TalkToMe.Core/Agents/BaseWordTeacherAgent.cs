using TalkToMe.Core.Builders;
using TalkToMe.Core.Enums;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Models;
using Newtonsoft.Json;

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

            return new CoreResponse { Response = JsonConvert.SerializeObject(response) };
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

            var response = JsonConvert.DeserializeObject<VocabularyChatResult>(agentResponse.Response)
                           ?? new VocabularyChatResult
                           {
                               response = "Error processing request.",
                               success = false,
                               status = VocabularyChatSessionStatus.Error
                           };

            response.status = _currentSession?.Status ?? VocabularyChatSessionStatus.Error;
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
            var str = new StringBuilder($"You are an AI {Language} language tutor helping users practice vocabulary through natural conversations.");
            str.AppendLine($"The word for practice is: {_currentSession?.CurrentWord}.");
            str.AppendLine("Follow this structured process: ");
            str.AppendLine("- Don't say any opening words.");
            str.AppendLine("- Introduce the word naturally in a sentence. Use it in a meaningful way so the user understands how it’s used in context.");
            str.AppendLine("- Ask an open-ended question that encourages the user to respond using the word.");
            str.AppendLine("- Highlight the practicing word in bold in your responses to make it more visible.");
            str.AppendLine();
            str.AppendLine("IMPORTANT INSTRUCTION: Your entire response **MUST** be a valid JSON object and **MUST NOT** contain any text outside of it.");
            str.AppendLine();
            str.AppendLine("Return the response in **exactly** the following JSON format, and nothing else:");
            str.AppendLine("{");
            str.AppendLine("  \"response\": \"Return the text of your response\",");
            str.AppendLine("  \"success\": true // Return true if correct, false otherwise");
            str.AppendLine("}");
            str.AppendLine();

            return str.ToString();
        }

        private string GetEvaluationPromt()
        {
            var str = new StringBuilder($"You are an AI {Language} language tutor helping users practice vocabulary through natural conversations.");
            str.AppendLine($"The word for practice is: {_currentSession?.CurrentWord}.");
            str.AppendLine("Evaluate the user’s response using this word: ");
            str.AppendLine("- If correct, praise them");
            str.AppendLine("- If incorrect, gently correct their sentence and explain the mistake. Ask them to try again");
            str.AppendLine("- Highlight the practicing word in bold in your responses to make it more visible.");
            str.AppendLine();
            str.AppendLine("IMPORTANT INSTRUCTION: Your entire response **MUST** be a valid JSON object and **MUST NOT** contain any text outside of it.");
            str.AppendLine();
            str.AppendLine("Return the response in **exactly** the following JSON format, and nothing else:");
            str.AppendLine("{");
            str.AppendLine("  \"response\": \"Return the text of your response\",");
            str.AppendLine("  \"success\": true // Return true if the sentence is correct and user uses the word correctly, false otherwise");
            str.AppendLine("}");
            str.AppendLine();

            return str.ToString();
        }

        private void UpdateSessionState(VocabularyChatResult response)
        {
            if (!response.success || _currentSession is null) return;

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
