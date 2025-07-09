using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DACS2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JudgeController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public JudgeController(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitCode([FromBody] JudgeRequest request)
        {
            var apiKey = _configuration["Judge0:ApiKey"];
            var apiHost = _configuration["Judge0:Host"];
            var judgeUrl = "https://judge0-ce.p.rapidapi.com/submissions?base64_encoded=false&wait=true";

            var payload = new
            {
                source_code = request.SourceCode,
                language_id = 63, // Node.js
                expected_output = request.ExpectedOutput
            };

            var json = JsonSerializer.Serialize(payload);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, judgeUrl)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            httpRequest.Headers.Add("X-RapidAPI-Key", apiKey);
            httpRequest.Headers.Add("X-RapidAPI-Host", apiHost);

            var response = await _httpClient.SendAsync(httpRequest);
            var responseBody = await response.Content.ReadAsStringAsync();

            return Ok(responseBody);
        }
    }

    public class JudgeRequest
    {
        public string SourceCode { get; set; }
        public string ExpectedOutput { get; set; }
    }
}
