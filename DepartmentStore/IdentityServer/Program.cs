using IdentityServer4.Models;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//++++++++++++++++ IdentityServer4++++++++++++++++
builder.Services.AddIdentityServer()
    .AddInMemoryApiScopes(new ApiScope[]
    {
        new ApiScope("ProductService_5000"), // API Scope cho ProductService
        new ApiScope("UserService_5002")
        // add more scopes here
    })
    .AddInMemoryClients(new Client[]
    {
        new Client
        {
            ClientId = "Product",
            AllowedGrantTypes = GrantTypes.ClientCredentials, // Sử dụng client_credentials grant type
            ClientSecrets = {new Secret("secret".Sha256())},  // Secret của client
            AllowedScopes = {"ProductService_5000"}           // Cho phép truy cập scope của ProductService
        },
        new Client
        {
            ClientId = "User",
            AllowedGrantTypes = GrantTypes.ClientCredentials, // Sử dụng client_credentials grant type
            ClientSecrets = {new Secret("secret".Sha256())},  // Secret của client
            AllowedScopes = {"UserService_5002"}
        }
        // add more clients here if needed
    })
    .AddDeveloperSigningCredential(); // Chỉ sử dụng cho phát triển, không dùng trong sản phẩm thực tế
//+++++++++++++++++++++++++++++++++++++++++++++++++

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
app.UseAuthentication();  // Quan trọng để bật cơ chế xác thực
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
