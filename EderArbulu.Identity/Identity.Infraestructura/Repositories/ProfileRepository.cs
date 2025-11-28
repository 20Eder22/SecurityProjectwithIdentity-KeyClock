using System.Security.Claims;
using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infraestructura
{
    public class ProfileRepository(UserManager<ApplicationUser> userManager): IProfileService
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await userManager.GetUserAsync(context.Subject);

            if (user is null) throw new ArgumentNullException();

            var claims = new List<Claim>
            {
                new(JwtClaimTypes.Email, user.Email!),
                new(JwtClaimTypes.Name, user.UserName!),
                new(JwtClaimTypes.Subject, user.Id)
            };

            var roles = await userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await userManager.GetUserAsync(context.Subject);
            context.IsActive = user != null;
        }
    }
}
