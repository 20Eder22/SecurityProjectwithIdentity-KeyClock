namespace Services
{
    public class ConfigService(ApiHttpClient api)
    {
        public async Task<string> ObtenerAccesosAsync()
        {
            var response = await api.GetAsync("https://localhost:7287/api/config/accesos");

            if (!response.IsSuccessStatusCode)
                throw new UnauthorizedAccessException();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> EnviarMailAsync()
        {
            var response = await api.PostAsync<string>("https://localhost:7287/api/config/SendMail", "");

            if (!response.IsSuccessStatusCode)
                throw new UnauthorizedAccessException();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
