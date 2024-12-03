using Microsoft.EntityFrameworkCore;

namespace ProductService_5000.Models
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<CategoryProduct> CategoryProducts { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p => p.CategoryProduct)
                .WithMany(c=>c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Image>()
                .HasOne(p=>p.Product)
                .WithMany(c=>c.Images)
                .HasForeignKey(i=>i.IdProduct)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Batch>()
                .HasOne(p => p.Product)
                .WithMany(b => b.Batches)
                .HasForeignKey(b => b.IdProduct);

            modelBuilder.Entity<Cart>()
                .HasOne(m => m.Product)
                .WithMany(m => m.Carts)
                .HasForeignKey(m => m.IdProduct);
        }
    }
}
