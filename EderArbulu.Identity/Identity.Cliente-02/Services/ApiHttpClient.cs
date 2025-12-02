namespace Services
{
    using Blazored.LocalStorage;
    using System.Net.Http.Headers;
    
    public class ApiHttpClient(HttpClient http, ILocalStorageService storage)
    {
        private async Task AddTokenAsync()
        {
            var token = await storage.GetItemAsync<string>("access_token");

            if (!string.IsNullOrWhiteSpace(token))
            {
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            await AddTokenAsync();
            return await http.GetAsync(url);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
        {
            await AddTokenAsync();

            var response = await http.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException();
            }

            return response;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            await AddTokenAsync();
            return await http.DeleteAsync(url);
        }
    }

}