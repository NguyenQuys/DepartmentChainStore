using IdentityServer4.Models;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//+++++++++ IdentityServer4 +++++++++
builder.Services.AddIdentityServer()
    .AddInMemoryApiResources(new ApiResource[]
    {
        new ApiResource("ProductService_5000", "Product Service")
        {
            Scopes = { "ProductService_5000" }
        }
    })
    .AddInMemoryApiScopes(new ApiScope[] {
        new ApiScope("ProductService_5000", "Access Product Service"),
        new ApiScope("UserService_5002", "Access User Service")
    })
    .AddInMemoryClients(new Client[] {
        new Client {
            ClientId = "Product",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedScopes = { "ProductService_5000" }
        },
        new Client {
            ClientId = "User",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedScopes = { "UserService_5002", "ProductService_5000" }
        }
    })
    .AddDeveloperSigningCredential();


//+++++++++++++++++++++++++++++++++++++++++

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use IdentityServer middleware
app.UseIdentityServer();

// Use authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
