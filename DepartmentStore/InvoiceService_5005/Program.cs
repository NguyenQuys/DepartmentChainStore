using APIGateway.Utilities;
using InvoiceService_5005.InvoiceModels;
using InvoiceService_5005.NewFolder;
using InvoiceService_5005.Services;
using Microsoft.EntityFrameworkCore;
using Ocelot.Values;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(InvoiceMapper));

builder.Services.AddDbContext<InvoiceDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("InvoiceDBConnection")));

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
