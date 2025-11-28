using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infraestructura
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddInfraestructura(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("IdentityDb"));
            });

            services.AddScoped<ProfileRepository>();

            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer().AddAspNetIdentity<ApplicationUser>()
                .AddInMemoryIdentityResources(ResourceConfig.IdentityResources)
                .AddInMemoryApiScopes(ResourceConfig.ApiScopes)
                .AddInMemoryClients(ResourceConfig.Clients)
                .AddProfileService<ProfileRepository>()
                .AddDeveloperSigningCredential();

            return services;
        }
    }
}
