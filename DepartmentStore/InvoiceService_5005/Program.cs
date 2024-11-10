using InvoiceService_5005.InvoiceModels;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<InvoiceDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("InvoiceDBConnection"))); 

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
