using System.Net.Http.Json;

namespace Services
{
    public class TokenService(HttpClient http)
    {
        public async Task<TokenResponse?> LoginAsync(string username, string password)
        {
            var endpoint = "https://localhost:7074/connect/token";

            var data = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "client-02" },
                { "client_secret", "secret" },
                { "username", username },
                { "password", password },
                { "scope", "roles email profile" }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new FormUrlEncodedContent(data)
            };

            var response = await http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<TokenResponse>();
        }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }
}
