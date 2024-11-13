using InvoiceService_5005.InvoiceModels;
using InvoiceService_5005.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<InvoiceDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("InvoiceDBConnection")));

builder.Services.AddScoped<IS_Invoice, S_Invoice>();
builder.Services.AddScoped<IS_Shipping, S_Shipping>();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
