using Microsoft.EntityFrameworkCore;

namespace InvoiceService_5005.InvoiceModels
{
	public class InvoiceDbContext : DbContext
	{
		public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : base(options)
		{
		}

		// DbSets cho các thực thể
		public DbSet<Invoice> Invoices { get; set; }
		public DbSet<Invoice_Product> Invoice_Products { get; set; }
		public DbSet<PaymentMethod> PaymentMethods { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Invoice_Product>()
				.HasOne(ip => ip.Invoice)
				.WithMany(i => i.Invoice_Products)
				.HasForeignKey(ip => ip.IdInvoice)
				.OnDelete(DeleteBehavior.Cascade);  // Xóa invoice sẽ xóa invoice products

			modelBuilder.Entity<Invoice>()
				.HasOne(i => i.PaymentMethod)
				.WithMany(pm => pm.Invoices)
				.HasForeignKey(i => i.IdPaymentMethod)
				.OnDelete(DeleteBehavior.SetNull);  // Khi xóa PaymentMethod, giữ lại Invoice nhưng không có PaymentMethod
		}
	}
}
