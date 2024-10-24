using IdentityServer4.Models;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureServices(builder);

var app = builder.Build();

// Middleware pipeline
ConfigureMiddleware(app);

app.Run();

void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddControllersWithViews();

    // IdentityServer configuration
    builder.Services.AddIdentityServer()
        .AddInMemoryApiResources(new[]
        {
            new ApiResource("ProductService_5000", "Product Service")
            {
                Scopes = { "ProductService_5000" }
            }
        })
        .AddInMemoryApiScopes(new[]
        {
            new ApiScope("ProductService_5000", "Access Product Service"),
            new ApiScope("UserService_5002", "Access User Service")
        })
        .AddInMemoryClients(new[]
        {
            new Client
            {
                ClientId = "User",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "UserService_5002", "ProductService_5000" }
            }
        })
        .AddDeveloperSigningCredential();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins", policy =>
        {
            policy.WithOrigins("https://localhost:7076")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

}

void ConfigureMiddleware(WebApplication app)
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    app.UseCors("AllowAllOrigins");

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseIdentityServer();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
}
