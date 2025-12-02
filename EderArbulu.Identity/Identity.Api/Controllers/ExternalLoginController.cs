namespace Identity.Api.Controllers
{
    namespace Identity.Api.Controllers
    {
        using Duende.IdentityServer.Models;
        using Duende.IdentityServer.Services;
        using Duende.IdentityServer.Stores;
        using Microsoft.AspNetCore.Authentication;
        using Microsoft.AspNetCore.Mvc;
        using System.Security.Claims;

        [Route("external")]
        public class ExternalLoginController(
            ITokenService tokenService,
            IClientStore clientStore)
            : Controller
        {

            [HttpGet("login/{provider}")]
            public IActionResult Login(string provider, string returnUrl = "/")
            {
                provider = NormalizeProvider(provider);

                var props = new AuthenticationProperties
                {
                    RedirectUri = Url.Action("Callback", new { provider, returnUrl })
                };

                return Challenge(props, provider);
            }

            [HttpGet("callback")]
            public async Task<IActionResult> Callback(string provider, string returnUrl = "/")
            {
                provider = NormalizeProvider(provider);

                var result = await HttpContext.AuthenticateAsync(provider);

                if (!result.Succeeded || result.Principal == null)
                {
                    return Redirect("https://localhost:7230/login?error=external_login_failed");
                }
                
                var externalUserId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

                var externalEmail =
                    result.Principal.FindFirstValue(ClaimTypes.Email) ??
                    result.Principal.FindFirstValue("email") ??
                    "";

                var externalName =
                    result.Principal.FindFirstValue(ClaimTypes.Name) ??
                    result.Principal.FindFirstValue("name") ??
                    provider + "-user";

                var claims = new List<Claim>
                {
                    new("sub", externalUserId),
                    new("name", externalName),
                    new("email", externalEmail),
                    new("idp", provider.ToLower()) // "github" o "google"
                };
                
                var client = await clientStore.FindEnabledClientByIdAsync("client-02");

                var token = new Token
                {
                    Issuer = "https://localhost:7074",
                    Lifetime = client.AccessTokenLifetime,
                    Claims = claims,
                    ClientId = "client-02",
                    AccessTokenType = AccessTokenType.Jwt
                };

                var accessToken = await tokenService.CreateSecurityTokenAsync(token);

                // 🔵 Regresar a Blazor con JWT listo
                return Redirect(
                    $"https://localhost:7230/external-callback?access_token={accessToken}&returnUrl={Uri.EscapeDataString(returnUrl)}");
            }

            private string NormalizeProvider(string provider)
            {
                return provider.ToLower() switch
                {
                    "github" => "GitHub",
                    "google" => "Google",
                    "microsoft" => "Microsoft",
                    _ => throw new Exception($"Proveedor no soportado: {provider}")
                };
            }
        }
    }
}
