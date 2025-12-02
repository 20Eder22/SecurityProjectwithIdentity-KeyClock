namespace Identity.Api.Controllers
{
    using Duende.IdentityServer.Models;
    using Duende.IdentityServer.Services;
    using Duende.IdentityServer.Stores;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    [Route("external")]
    public class ExternalLoginController(ITokenService tokenService, IClientStore clientStore)
        : Controller
    {
        [HttpGet("github")]
        public IActionResult LoginWithGithub(string returnUrl = "/")
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GithubCallback", new { returnUrl })
            };

            return Challenge(props, "GitHub");
        }

        [HttpGet("github-callback")]
        public async Task<IActionResult> GithubCallback(string returnUrl = "/")
        {
            var result = await HttpContext.AuthenticateAsync("GitHub");

            if (!result.Succeeded || result.Principal == null)
                return Redirect("https://localhost:7230/login?error=external_login_failed");

            var externalUserId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var externalEmail = result.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
            var externalName = result.Principal.FindFirstValue(ClaimTypes.Name) ?? "github-user";

            var claims = new List<Claim>
            {
                new("sub", externalUserId),
                new("name", externalName),
                new("email", externalEmail),
                new("idp", "github")
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

            return Redirect(
                $"https://localhost:7230/github-callback?access_token={accessToken}&returnUrl={Uri.EscapeDataString(returnUrl)}");
        }
    }
}
