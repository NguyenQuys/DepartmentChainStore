using APIGateway.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using ProductService_5000.Mapper;
using ProductService_5000.Models;
using ProductService_5000.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// EPPLUS
ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Hoặc LicenseContext.Commercial nếu bạn có giấy phép thương mại

// Configure the DbContext with the connection string from appsettings.json
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductDBConnection")));

builder.Services.AddAutoMapper(typeof(ProductMapper));
builder.Services.AddAutoMapper(typeof(BatchMapper));

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001"; // IdentityServer URL
        options.Audience = "ProductService_5000";
        options.RequireHttpsMetadata = false; // Set to true in production

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],        // Correct case: "Jwt" instead of "jwt"
            ValidAudience = builder.Configuration["Jwt:Audience"],    // Ensure correct section name
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])),
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

builder.Services.AddHttpClient("ProductService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7076");
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register the product service
builder.Services.AddScoped<IS_Product, S_Product>();
builder.Services.AddScoped<IS_Batch, S_Batch>();
builder.Services.AddScoped<CurrentUserHelper>();

// Add Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware: Use HTTPS redirection and static files
app.UseHttpsRedirection();
app.UseStaticFiles();

// Middleware: Use routing
app.UseRouting();

// Enable authentication and authorization
app.UseAuthentication(); 
app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.Run();
