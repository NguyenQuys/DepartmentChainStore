using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load Ocelot configuration from the ocelot.json file
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Register JWT Bearer authentication
builder.Services.AddAuthentication(options =>
{
    // Use JWT as the default authentication scheme
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

// Register Ocelot services
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Middleware to use authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map a simple GET endpoint
app.Use(async (context, next) =>
{
    // Check if the request path is the root path
    if (context.Request.Path == "/")
    {
        // Redirect to /home/index
        //context.Response.Redirect("/User/Login");
        context.Response.Redirect("/Product/Index");
        return; // Short-circuit the pipeline
    }

    await next(); // Call the next middleware in the pipeline
});

// Use Ocelot to handle the API Gateway routing
await app.UseOcelot();

app.Run();
