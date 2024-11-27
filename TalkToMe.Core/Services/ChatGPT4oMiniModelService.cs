using Newtonsoft.Json;
using RestSharp;
using TalkToMe.Core.Configuration;
using TalkToMe.Core.Models.OpenAiModels;
using TalkToMe.Core.Models;
using TalkToMe.Core.Interfaces;

namespace TalkToMe.Core.Services
{
    public class ChatGPT4oMiniModelService : IAiModelService
    {
        private readonly OpenAiSettings _settings;
        private readonly RestClient _client;
        private bool _disposed;
        private readonly string _modelId;

        public ChatGPT4oMiniModelService(OpenAiSettings settings, string modelId)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            if (string.IsNullOrEmpty(_settings.BaseUrl)) throw new ArgumentException("Base URL must be provided.", nameof(settings.BaseUrl));
            if (string.IsNullOrEmpty(_settings.ApiKey)) throw new ArgumentException("API key must be provided.", nameof(settings.ApiKey));
            if (string.IsNullOrEmpty(modelId)) throw new ArgumentNullException("modelId");

            _client = new RestClient(_settings.BaseUrl);
            _modelId = modelId;
        }

        public string ModelId 
        {
            get { return _modelId; }
        }

        public async Task<CoreResponse> SendMessageAsync(CoreRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var restRequest = new RestRequest("chat/completions", Method.Post);
            restRequest.AddHeader("Authorization", $"Bearer {_settings.ApiKey}");

            restRequest.AddJsonBody(new
            {
                model = _modelId,
                messages = new[]
                {
                    new { role = "system", content = request.SystemInstruction },
                    new { role = "user", content = request.Prompt }
                }
            });

            var response = await _client.ExecuteAsync(restRequest);
            if (response.IsSuccessful)
            {
                dynamic result = JsonConvert.DeserializeObject<OpenAiResponse>(response.Content);
                return new CoreResponse
                {
                    Response = result.Choices[0].Message.Content
                };
            }
            else
            {
                throw new Exception($"Error: {response.StatusCode}, {response.Content}");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _client?.Dispose();
            }

            _disposed = true;
        }
    }
}
