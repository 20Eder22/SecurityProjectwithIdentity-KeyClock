using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/SecurityEnvironment";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidAudience = "resource-utilitarios",
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:8080/realms/SecurityEnvironment",
            ValidateLifetime = true
        };

        options.Events = new JwtBearerEvents()
        {
            OnTokenValidated = context =>
            {
                var identity = context.Principal?.Identity as ClaimsIdentity;
                if (identity is null)
                    return Task.CompletedTask;

                var resourceAccess = context.Principal!.FindFirst("resource_access")!.Value;
                if (!string.IsNullOrEmpty(resourceAccess))
                {
                    using var doc = JsonDocument.Parse(resourceAccess);
                    if (doc.RootElement.TryGetProperty("resource-utilitarios", out var apiRes))
                    {
                        if (apiRes.TryGetProperty("roles", out var roles))
                        {
                            foreach (var role in roles.EnumerateArray())
                            {
                                var rolName = role.GetString();
                                if (!string.IsNullOrEmpty(rolName))
                                {
                                    identity.AddClaim(new Claim(ClaimTypes.Role, rolName));
                                }
                            }

                        }
                    }
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireReadRole", policy =>
    {
        policy.RequireRole("READ");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/SendEmail", () =>
{
    var mensaje = "Mail enviado";
    return Results.Ok(mensaje);
}).RequireAuthorization();

app.MapControllers();

app.Run();
