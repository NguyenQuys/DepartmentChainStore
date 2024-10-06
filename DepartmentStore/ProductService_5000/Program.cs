using Microsoft.EntityFrameworkCore;
using ProductService_5000.Helper;
using ProductService_5000.Models;
using ProductService_5000.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Set up interfaces
builder.Services.AddScoped<IS_Product, S_Product>();

// Configure DbContext
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Jwt authentication
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.Authority = "https://localhost:5001";
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateAudience = false
    };
});

// Add authorization with policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProductServicePolicy", policy =>
    {
        policy.RequireClaim("scope", "ProductService");
    });
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

// Map default route for controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=GetAll}/{id?}");

app.Run();
