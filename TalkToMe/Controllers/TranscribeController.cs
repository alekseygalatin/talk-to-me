using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.TranscribeService;
using Amazon.TranscribeService.Model;
using Amazon.Polly;
using Amazon.Polly.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Amazon.BedrockAgentRuntime;
using Amazon.BedrockAgentRuntime.Model;
using Microsoft.Extensions.Caching.Memory;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using PayloadPart = Amazon.BedrockAgentRuntime.Model.PayloadPart;

namespace TalkToMe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranscribeController : ControllerBase
    {
        private readonly string s3BucketName = "talktomebucket";
        private readonly string transcribeJobName = "transcription-job";
        private readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private readonly AmazonS3Client _s3Client;
        private readonly AmazonTranscribeServiceClient _transcribeClient;
        private readonly AmazonPollyClient _pollyClient;
        private readonly MemoryCache _cache;
        private static AmazonBedrockAgentRuntimeClient _bedrockRuntime = new AmazonBedrockAgentRuntimeClient(RegionEndpoint.USEast1);

        private readonly string _promt =
            "You are a conversational assistant whose goal is to help users practice simple English conversations. Your role is to be proactive, ask engaging questions, and provide detailed responses that encourage further interaction. You should guide the conversation through various common topics such as daily life, hobbies, work, travel, and personal preferences. Keep your tone friendly and approachable, offering explanations and examples where needed to enhance understanding.\n\nEnsure your responses are long and detailed, with natural language flow. Always include follow-up questions that relate to the user's answers or move the conversation forward into new, but relevant topics. Here's the structure you should follow:\n\nStart the conversation with an engaging question that invites a personal response (e.g., \"How has your day been so far?\" or \"What are you up to today?\")\nProvide a long, informative answer to each of the user’s replies. Explain your thoughts, and if relevant, add details like why or how something happens. Add interesting facts or examples to keep the conversation engaging.\nAsk follow-up questions to keep the conversation flowing. Tailor your questions to what the user says. If they mention their hobbies, ask more about those. If they talk about travel, ask them about their favorite destination.\nAlways offer to clarify any challenging words or phrases and encourage the user to ask questions if they don't understand something.\nExample Conversation:\n\nYou:\n\"Hi there! How’s your day going so far? Have you been busy with anything interesting?\"\n\nUser:\n\"My day has been okay, just a bit busy with work.\"\n\nYou (Long response):\n\"It sounds like you’ve had a productive day! It’s good to stay busy, though it can sometimes be overwhelming. I know work can take up a lot of energy, especially when there are deadlines or important tasks to complete. What kind of work do you do? Are there any particular projects you’re focusing on right now that are keeping you occupied?\"\n\nFollow-up question:\n\"Do you usually enjoy staying busy, or do you prefer a slower, more relaxed pace? Some people thrive in a fast-paced environment, while others need quiet time to recharge. How do you balance your work and personal life?\"\n\nBy following this structure, the model should be able to engage in conversations that help the user practice their English in a supportive and detailed manner.";

        public TranscribeController()
        {
            _s3Client = new AmazonS3Client(bucketRegion);
            _transcribeClient = new AmazonTranscribeServiceClient(bucketRegion);
            _pollyClient = new AmazonPollyClient(bucketRegion);
            _cache = new MemoryCache(new MemoryCacheOptions());
        }
        
        [HttpPost("process-text")]
        public async Task<IActionResult> ProcessText([FromBody] string text, [FromQuery] string sessionId)
        {
            // Step 4: Send the transcription text to AWS Bedrock with the provided prompt and session context
            string bedrockResponse = await SendTextToBedrock(text);

            // Step 3: Convert the transcribed text back into an MP3 using Polly
            var outputFilePath = await ConvertTextToSpeech(bedrockResponse);

            // Return the path or file download
            return outputFilePath;
        }

        [HttpPost("process-audio")]
        public async Task<IActionResult> ProcessAudio([FromForm] IFormFile audioFile, [FromQuery] string sessionId)
        {
            if (audioFile == null || audioFile.Length == 0)
                return BadRequest("Invalid audio file.");

            // Step 1: Upload the audio file to S3
            var mp3InputFileKey = Guid.NewGuid().ToString() + Path.GetExtension(audioFile.FileName);
            await UploadFileToS3(audioFile, mp3InputFileKey);

            // Step 2: Transcribe the MP3 file to text
            string transcriptText = await TranscribeMp3ToText(mp3InputFileKey);

            // Step 4: Send the transcription text to AWS Bedrock with the provided prompt and session context
            string bedrockResponse = await SendTextToBedrock(transcriptText);

            // Step 3: Convert the transcribed text back into an MP3 using Polly
            var outputFilePath = await ConvertTextToSpeech(bedrockResponse);

            // Return the path or file download
            return outputFilePath;
        }

        private async Task UploadFileToS3(IFormFile audioFile, string fileKey)
        {
            var fileTransferUtility = new TransferUtility(_s3Client);

            using (var newMemoryStream = new MemoryStream())
            {
                audioFile.CopyTo(newMemoryStream);
                await fileTransferUtility.UploadAsync(newMemoryStream, s3BucketName, fileKey);
            }

            Console.WriteLine("File uploaded to S3 successfully.");
        }
        
        private async Task<string> SendTextToBedrock(string chat)
        {
            var response = await _bedrockRuntime.InvokeAgentAsync(new InvokeAgentRequest
            {
                AgentAliasId = "Q4NJBFBPO0",
                AgentId = "DATDG2T7FI",
                InputText = chat,
                SessionId = "123456789"
            });
            
            if(response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                MemoryStream output = new MemoryStream();
                foreach(PayloadPart item in response.Completion)
                {
                    item.Bytes.CopyTo(output);
                }
                var result = Encoding.UTF8.GetString(output.ToArray());

                return result;
            }

            return "";
        }

        private async Task<string> TranscribeMp3ToText(string mp3InputFileKey)
        {
            var startTranscriptionJobRequest = new StartTranscriptionJobRequest
            {
                TranscriptionJobName = transcribeJobName + Guid.NewGuid(),
                LanguageCode = "en-US",
                MediaFormat = "wav",
                Media = new Media
                {
                    MediaFileUri = $"s3://{s3BucketName}/{mp3InputFileKey}"
                },
                OutputBucketName = s3BucketName
            };

            var transcriptionJobResponse = await _transcribeClient.StartTranscriptionJobAsync(startTranscriptionJobRequest);
            Console.WriteLine("Transcription job started...");

            TranscriptionJob transcriptionJob;
            do
            {
                await Task.Delay(2000); // Wait for job completion
                transcriptionJob = (await _transcribeClient.GetTranscriptionJobAsync(new GetTranscriptionJobRequest
                {
                    TranscriptionJobName = startTranscriptionJobRequest.TranscriptionJobName
                })).TranscriptionJob;
            } while (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.IN_PROGRESS);

            if (transcriptionJob.TranscriptionJobStatus == TranscriptionJobStatus.COMPLETED)
            {
                // Download the transcription result from S3
                string transcriptFileKey = $"{startTranscriptionJobRequest.TranscriptionJobName}.json";
                string json = await GetTranscriptionTextFromS3(transcriptFileKey);
                dynamic transcriptionResult = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                // Extract the transcript text
                string transcriptText = transcriptionResult.results.transcripts[0].transcript;
                return transcriptText;
            }

            throw new Exception("Transcription job failed.");
        }
        
        

        private async Task<string> GetTranscriptionTextFromS3(string transcriptFileKey)
        {
            var response = await _s3Client.GetObjectAsync(s3BucketName, transcriptFileKey);
            using (StreamReader reader = new StreamReader(response.ResponseStream))
            {
                string json = await reader.ReadToEndAsync();
                // Extract transcription text from JSON (simplified parsing)
                return json; // Parse it further as needed
            }
        }

        private async Task<IActionResult> ConvertTextToSpeech(string text)
        {
            var synthesizeSpeechRequest = new SynthesizeSpeechRequest
            {
                OutputFormat = OutputFormat.Mp3,
                Text = text,
                VoiceId = "Joanna"
            };

            var synthesizeSpeechResponse = await _pollyClient.SynthesizeSpeechAsync(synthesizeSpeechRequest);

            //string outputFilePath = $"output-{Guid.NewGuid()}.mp3";
            using var fileStream = new MemoryStream();
            await synthesizeSpeechResponse.AudioStream.CopyToAsync(fileStream);

            byte[] wavArray = fileStream.ToArray();

            return File(wavArray, "audio/wav", "convertedAudio.wav");
        }
    }
}
