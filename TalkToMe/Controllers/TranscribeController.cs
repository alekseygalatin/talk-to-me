using Amazon.Lambda.APIGatewayEvents;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace TalkToMe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranscribeController : ControllerBase
    {
        [HttpPost("process-text")]
        public async Task<APIGatewayHttpApiV2ProxyResponse> ProcessText([FromBody] string text, [FromQuery] string sessionId)
        {
            var transcribeHandler = new TranscribeHandler();
            var result = await transcribeHandler.ProcessText(new APIGatewayHttpApiV2ProxyRequest
            {
                RequestContext = new APIGatewayHttpApiV2ProxyRequest.ProxyRequestContext
                {
                    Http = new APIGatewayHttpApiV2ProxyRequest.HttpDescription
                    {
                        Method = "POST"
                    }
                },
                Headers = new Dictionary<string, string>()
                {
                    {"authorization", "eyJraWQiOiJDU2x2ZmdBem9OZk9rMXV6OVFkQmxYbVJYNkhkVVgrVnBrT1g4UWZFcTlnPSIsImFsZyI6IlJTMjU2In0.eyJhdF9oYXNoIjoiVUplYU03SzN2cnlWQnM4cWJWckItZyIsInN1YiI6ImE0YzgwNDY4LWIwNTEtNzA2OS1lZjFmLWUwNjRjY2FlMjhhZiIsImlzcyI6Imh0dHBzOlwvXC9jb2duaXRvLWlkcC51cy1lYXN0LTEuYW1hem9uYXdzLmNvbVwvdXMtZWFzdC0xX3dhbERDcE5jSyIsImNvZ25pdG86dXNlcm5hbWUiOiJhbGV4Iiwib3JpZ2luX2p0aSI6ImEzZWExYWI2LTNjNmUtNGFkNi04MDg2LTNiZDkzMzA3MmQzNSIsImF1ZCI6IjdvOHRxbHQydWNpaHFzYnR0aGZvcGM5ZDRwIiwiZXZlbnRfaWQiOiI5OTQ2MzljMy1jZDViLTRkMWYtOTU1Ni0wMjY1ZGYxZGVjYTAiLCJ0b2tlbl91c2UiOiJpZCIsImF1dGhfdGltZSI6MTczMTQwNzkxNywiZXhwIjoxNzMxNDExNTE3LCJpYXQiOjE3MzE0MDc5MTcsImp0aSI6IjhjZTZiNGRmLTY4NGItNDA5Zi1hYWJlLTg4MmNjZTYzOGU1YyJ9.asktbVotsU1A3jbGu4xQ483kssxhst1_LSNFRcZl17wm6rTpo81zOjp-SVwwfndayHi1oXYT5c-u46ImgtUVUcDHbHX_3kj4Jzk9dfGTRSCCqxOVB_S0jrUcCPZhybtlhPRQ9n2A3WHZwXb0ffjU2_qThzU3LoukPM8gbCzoq6XfhfmoFIRpfYsqKLN6bdcVrygX_wfE9VSf8wVKdiVw3yIgaE0ISuiDrsyxjWaDrHzxLlVpDS-z_qnf11E2aKqG9YhP-4myIyQV1q8cXsCDxuHxxzoqO5jFRd-xW9pPou-mYbjFEnvcoLkXRJT6cUHH5tAypI3Ht6Q1lnu_yL8OYA"}
                },
                Body = JsonConvert.SerializeObject(new
                {
                    text = text
                })
            });

            return result;
        }
    }
}
