using IdentityServer.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        options.Authority = "https://localhost:5001"; // Make sure this URL is correct
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            // Configure issuer and audience as needed, e.g.:
            ValidIssuer = builder.Configuration["jwt:Issuer"],
            ValidAudience = builder.Configuration["jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt:Key"])),
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

// Register your services
builder.Services.AddHttpClient(); // For calling API to ProductService
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IS_User, S_User>();
builder.Services.AddScoped<ISendMailSMTP, SendMailSMTP>();
builder.Services.AddScoped<IOTP_Verify, OTP_Verify>();
builder.Services.AddScoped<IS_ProductFromUser, S_ProductFromUser>();
builder.Services.AddScoped<CurrentUserHelper>();  // Register CurrentUserHelper
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserServicePolicy", policy =>
    {
        policy.RequireClaim("scope", "UserService");
    });
});

// Register JwtHelper as a singleton
builder.Services.AddSingleton<IJwtHelper, JwtHelper>();

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

app.UseAuthentication(); // Ensure this is before UseAuthorization
app.UseAuthorization();

// Map default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();

// Method to retrieve the connection string
string GetConnectionString(string name)
{
    return builder.Configuration.GetConnectionString(name);
}
