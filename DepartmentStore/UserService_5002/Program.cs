using IdentityModel.Client;
using IdentityServer.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService_5002.Models;
using UserService_5002.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the DbContext with the connection string
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(GetConnectionString("DefaultConnection")));

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001"; // IdentityServer URL
        options.Audience = "UserService_5002"; // This should match the API scope
        options.RequireHttpsMetadata = false; // Set to true in production
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://localhost:5001",
            ValidAudience = "UserService_5002",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt:Key"])),
            ClockSkew = TimeSpan.Zero
        };
    });

// Register your services (same as before)
builder.Services.AddHttpClient("IdentityServer", client =>
{
    client.BaseAddress = new Uri("https://localhost:5001/");
});

builder.Services.AddHttpClient("ProductService", client =>
{
    client.BaseAddress = new Uri("https://localhost:5000/");
});

builder.Services.AddSingleton<IDiscoveryCache>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return new DiscoveryCache("https://localhost:5001", () => factory.CreateClient());
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IS_User, S_User>();
builder.Services.AddScoped<ISendMailSMTP, SendMailSMTP>();
builder.Services.AddScoped<IOTP_Verify, OTP_Verify>();
builder.Services.AddScoped<IS_ProductFromUser, S_ProductFromUser>();
builder.Services.AddScoped<CurrentUserHelper>();

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserServicePolicy", policy =>
    {
        policy.RequireClaim("scope", "UserService_5002");
    });
    options.AddPolicy("RequireAdminRole", policy =>
    {
        policy.RequireAuthenticatedUser();
        //policy.RequireRole("1");
    });
});

builder.Services.AddSingleton<IJwtHelper, JwtHelper>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowProductService", policy =>
    {
        policy.WithOrigins("https://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Use CORS before authentication and authorization
app.UseCors("AllowProductService");

app.UseAuthentication();
app.UseAuthorization();

// Configure routing to support areas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Method to retrieve the connection string
string GetConnectionString(string name)
{
    return builder.Configuration.GetConnectionString(name);
}
