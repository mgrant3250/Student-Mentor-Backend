using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace AugustaAlumniAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public JobsController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpGet("local")]
        public async Task<IActionResult> GetLocalJobs([FromQuery] string query = "developer", [FromQuery] string location = "Augusta, GA")
        {
            var apiKey = _configuration["RapidAPI:Key"];
            var url = $"https://jsearch.p.rapidapi.com/search?query={query}&location={Uri.EscapeDataString(location)}&page=1&num_pages=1";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-RapidAPI-Key", apiKey);
            request.Headers.Add("X-RapidAPI-Host", "jsearch.p.rapidapi.com");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to fetch job listings.");
            }

            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json");
        }

    }
}
