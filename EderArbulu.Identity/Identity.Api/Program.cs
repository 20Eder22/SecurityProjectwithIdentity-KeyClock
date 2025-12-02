using Identity.Infraestructura;

var corsPolicy = "blazorWasmPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddInfraestructura(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, policy =>
    {
        policy
            .WithOrigins("https://localhost:7230")  // tu Blazor WASM
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // IdentityServer lo requiere
    });
});

var app = builder.Build();

app.UseCors(corsPolicy);

app.UseIdentityServer();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DataSeeder.SeedAsync(services);
}

app.Run();
