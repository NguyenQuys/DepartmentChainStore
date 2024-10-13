using IdentityServer4.Models;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:5002") // Địa chỉ của UserService
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

//++++++++++++++++ IdentityServer4++++++++++++++++
builder.Services.AddIdentityServer()
    .AddInMemoryApiScopes(new ApiScope[]
    {
        new ApiScope("ProductService_5000"), // API Scope cho ProductService
        new ApiScope("UserService_5002")
    })
    .AddInMemoryClients(new Client[]
    {
        new Client
        {
            ClientId = "Product",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedScopes = { "ProductService_5000" }
        },
        new Client
        {
            ClientId = "User",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedScopes = { "UserService_5002" }
        }
    })
    .AddDeveloperSigningCredential(); // Sử dụng cho phát triển

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

app.UseCors("AllowSpecificOrigin"); // Sử dụng CORS

app.UseIdentityServer(); // Quan trọng để thêm IdentityServer middleware

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
