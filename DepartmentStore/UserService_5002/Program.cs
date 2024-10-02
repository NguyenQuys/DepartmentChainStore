using Microsoft.EntityFrameworkCore;
using ProductService_5000.Helper;
using UserService_5002.Models;

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

// Add Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserServicePolicy", policy =>
    {
        policy.RequireClaim("scope", "UserService");
    });
});

// Register the JwtHelper as a singleton
builder.Services.AddSingleton<JwtHelper>();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Method to retrieve the connection string
string GetConnectionString(string name)
{
    return builder.Configuration.GetConnectionString(name);
}
