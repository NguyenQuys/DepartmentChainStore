using APIGateway.Utilities;
using InvoiceService_5005.InvoiceModels;
using InvoiceService_5005.NewFolder;
using InvoiceService_5005.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Values;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(InvoiceMapper));

builder.Services.AddDbContext<InvoiceDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("InvoiceDBConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.Authority = "https://localhost:5001"; // IdentityServer URL
		options.Audience = "InvocieService_5005";
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

builder.Services.AddScoped<IS_Invoice, S_Invoice>();
builder.Services.AddScoped<IS_Shipping, S_Shipping>();
builder.Services.AddScoped<ISendMailSMTP, SendMailSMTP>();

builder.Services.AddHttpClient("ProductService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7076");
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
