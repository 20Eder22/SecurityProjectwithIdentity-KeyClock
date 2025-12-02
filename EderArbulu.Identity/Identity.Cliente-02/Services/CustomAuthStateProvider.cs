using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Services
{
    public class CustomAuthStateProvider(ILocalStorageService storage) : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await storage.GetItemAsync<string>("access_token");

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            JwtSecurityToken jwt;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                jwt = handler.ReadJwtToken(token);
            }
            catch
            {
                await LogoutAsync();
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var exp = jwt.Payload.Expiration;
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (exp < now)
            {
                await LogoutAsync();
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = jwt.Claims.ToList();

            var nameClaim = claims.FirstOrDefault(c => c.Type == "name");
            if (nameClaim != null)
            {
                claims.Add(new Claim(ClaimTypes.Name, nameClaim.Value));
            }

            var emailClaim = claims.FirstOrDefault(c => c.Type == "email");
            if (emailClaim != null)
            {
                claims.Add(new Claim(ClaimTypes.Email, emailClaim.Value));
            }

            var identity = new ClaimsIdentity(claims, "jwt");

            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
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
