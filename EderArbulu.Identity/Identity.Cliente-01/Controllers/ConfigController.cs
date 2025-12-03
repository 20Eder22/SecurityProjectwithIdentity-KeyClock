using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Identity.Cliente_01.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConfigController : ControllerBase
    {
        [HttpGet("Accesos")]
        public IEnumerable<string> Accesos()
        {
            return Enumerable.Range(1, 5).Select(i=>"Accesos " + i)
                .ToArray();
        }

        [HttpPost("SendMail")]
        public async Task<IActionResult> SendMail()
        {
            var client = new HttpClient();
            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", "client-utilitarios"},
                { "client_secret", "7p5hwtHUj0hBcT4srU5nxkqiWfIGKuR6"},
                { "grant_type", "client_credentials"}
            });

            var tokenResponse = await client.PostAsync("http://localhost:8080/realms/SecurityEnvironment/protocol/openid-connect/token", tokenRequest);
            var tokenJson = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync());
            var accessToken = tokenJson.RootElement.GetProperty("access_token").GetString();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var apiResponse = await client.GetAsync("https://localhost:7221/utilitarios-back/SendEmail");

            if (apiResponse is { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.Unauthorized })
            {
                return BadRequest("Acceso no autorizado");
            }

            var result = await apiResponse.Content.ReadAsStringAsync();
            return Ok(result);
        }
    }
}
