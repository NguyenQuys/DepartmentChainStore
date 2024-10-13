using IdentityServer.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductService_5000.Models;
using ProductService_5000.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Set up interfaces
builder.Services.AddScoped<IS_Product, S_Product>();
builder.Services.AddHttpContextAccessor(); // Add IHttpContextAccessor
builder.Services.AddScoped<CurrentUserHelper>();  // Register CurrentUserHelper

// Add Jwt authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = "https://localhost:5001"; // Địa chỉ của IdentityServer
    options.RequireHttpsMetadata = false; // Chỉ dùng cho phát triển
    options.Audience = "ProductService_5000"; // Scope của dịch vụ này
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
    {
        policy.RequireAuthenticatedUser();
        //policy.RequireRole("1");
    });
    options.AddPolicy("ProductServicePolicy", policy =>
    {
        policy.RequireClaim("scope", "ProductService");
    });

});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:5002") // Allow UserService
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

// Add JwtHelper as a singleton
builder.Services.AddSingleton<JwtHelper, JwtHelper>();

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

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowSpecificOrigin"); // communicate service

// Map default route for controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=GetAll}/{id?}");

app.Run();
