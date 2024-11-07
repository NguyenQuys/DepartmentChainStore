using Microsoft.EntityFrameworkCore;

namespace PromotionService_5004.Models
{
    public class PromotionDbContext : DbContext
    { 
        public PromotionDbContext(DbContextOptions<PromotionDbContext> options) : base(options) { }

        public DbSet<Promotion> Promions { get; set; }
        public DbSet<PromotionType> PromotionTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Promotion>()
                .HasOne(p => p.PromotionType)
                .WithMany(p => p.Promotions)
                .HasForeignKey(m => m.IdPromotionType);
        }
    }
}
