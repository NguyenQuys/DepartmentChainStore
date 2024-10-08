using IdentityServer.Utilities;
using Microsoft.EntityFrameworkCore;
using ProductService_5000.Services;
using UserService_5002.Models;
using UserService_5002.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the DbContext with the connection string
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(GetConnectionString("DefaultConnection")));

// Add JWT Authentication
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.Authority = "https://localhost:5001"; // Make sure this URL is correct
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateAudience = false
    };
});

builder.Services.AddHttpClient(); // Call API to productservice
builder.Services.AddHttpContextAccessor(); 
builder.Services.AddScoped<IS_User, S_User>();
builder.Services.AddScoped<ISendMailSMTP, SendMailSMTP>();
builder.Services.AddScoped<IOTP_Verify, OTP_Verify>();
// Product Service
builder.Services.AddScoped<IS_ProductFromUser, S_ProductFromUser>();
builder.Services.AddScoped<CurrentUserHelper>();  // <-- Register it here
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserServicePolicy", policy =>
    {
        policy.RequireClaim("scope", "UserService");
    });
});

// Register the JwtHelper as a singleton
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();

// Method to retrieve the connection string
string GetConnectionString(string name)
{
    return builder.Configuration.GetConnectionString(name);
}
