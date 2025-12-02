using Blazored.LocalStorage;

namespace Services
{
    using Microsoft.AspNetCore.Components.Authorization;
    using System.Security.Claims;

    public class CustomAuthStateProvider(ILocalStorageService storage) : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await storage.GetItemAsync<string>("access_token");

            if (string.IsNullOrEmpty(token))
            {
                // Usuario NO autenticado
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Usuario autenticado
            var identity = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, "usuario") // Puedes leer claims reales del token
            ], "jwt");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task SetTokenAsync(string token)
        {
            await storage.SetItemAsync("access_token", token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task LogoutAsync()
        {
            await storage.RemoveItemAsync("access_token");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }

}
