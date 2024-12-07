using APIGateway.Utilities;
using AspNetCoreRateLimit;
using BranchService_5003.Models;
using BranchService_5003.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Values;
using ProductService_5000.Mapper;
using System.Configuration;
using System.Text;
using UserService_5002.Helper;
using UserService_5002.Models;
using UserService_5002.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the DbContext with the connection string
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(GetConnectionString("UserDBConnection")));

builder.Services.AddDbContext<BranchDBContext>(options =>
    options.UseSqlServer(GetConnectionString("BranchDBConnection")));

// Cấu hình JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["jwt:Issuer"],            // Get Issuer from appsettings.json
        ValidAudience = builder.Configuration["jwt:Audience"],        // Get Audience from appsettings.json
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["jwt:Key"])),  // Get Key from appsettings.json
        ClockSkew = TimeSpan.Zero // Eliminate default clock skew of 5 minutes
    };

    // Automatically extract the JWT token from cookies
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

// Register your services (same as before)
builder.Services.AddHttpClient("APIGateway", client =>
{
    client.BaseAddress = new Uri("https://localhost:7076/");
});

//builder.Services.AddHttpClient("ProductService", client =>
//{
//    client.BaseAddress = new Uri("https://localhost:5000/");
//});

builder.Services.AddSingleton<IDiscoveryCache>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return new DiscoveryCache("https://localhost:7076", () => factory.CreateClient());
});

builder.Services.AddSession();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IS_User, S_User>();
builder.Services.AddScoped<ISendMailSMTP, SendMailSMTP>();
builder.Services.AddScoped<IOTP_Verify, OTP_Verify>();
builder.Services.AddScoped<IS_ProductFromUser, S_ProductFromUser>();
builder.Services.AddScoped<IS_Auth, S_Auth>(); // S_Auth depends on BranchDBContext
builder.Services.AddScoped<IS_Branch, S_Branch>();
builder.Services.AddScoped<IS_OTP, S_OTP>();
builder.Services.AddScoped<CurrentUserHelper>();

// Add Authorization
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("UserServicePolicy", policy =>
//    {
//        policy.RequireClaim("scope", "UserService_5002");
//    });
//    options.AddPolicy("RequireAdminRole", policy =>
//    {
//        policy.RequireAuthenticatedUser();
//        //policy.RequireRole("1");
//    });
//});

builder.Services.AddSingleton<IJwtHelper, JwtHelper>();

// Add CORS once and configure all policies
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowApiGateway",
            builder =>
            {
                builder.WithOrigins("https://localhost:7076")  // Cho phép API Gateway
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();  // Cho phép cookie được gửi
            });
});

// Rate limited
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

//+++++++++++++++++++++++ Mapper +++++++++++++++++++++++
builder.Services.AddAutoMapper(typeof(UserMapper));
//++++++++++++++++++++++++++++++++++++++++++++++++++++++

var app = builder.Build();

// Use the specific CORS policy needed in the middleware
app.UseCors("AllowAllOrigins");  // Use this if you need to allow all origins
app.UseCors("AllowSpecificOrigin");


app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None, // Required for cross-domain
});

// Other middleware and configurations remain the same
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// limit rated
app.UseIpRateLimiting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


// Method to retrieve the connection string
string GetConnectionString(string name)
{
    return builder.Configuration.GetConnectionString(name);
}
