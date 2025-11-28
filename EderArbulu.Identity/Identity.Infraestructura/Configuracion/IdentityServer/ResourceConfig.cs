
using Duende.IdentityServer.Models;

namespace Identity.Infraestructura
{
    public static class ResourceConfig
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new(
                name: "roles",
                userClaims: ["role"],
                displayName: "Roles"
            )
        ];

        public static IEnumerable<ApiScope> ApiScopes =>
        [
            new(
                name: "api-01",
                displayName: "Api utilitarios"
            )
        ];

        public static IEnumerable<Client> Clients =>
        [
            new()
            {
                ClientId = "client-01",
                ClientName = "Cliente de utilitarios",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = [new Secret("secret".Sha256())],
                AllowedScopes = ["api-01", "openId", "profile", "email", "roles"]
            }
        ];
    }
}
