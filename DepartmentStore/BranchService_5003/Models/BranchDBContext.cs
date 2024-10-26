using Microsoft.EntityFrameworkCore;

namespace BranchService_5003.Models
{
    public class BranchDBContext : DbContext
    {
        public BranchDBContext(DbContextOptions<BranchDBContext> options) : base(options) { }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Product_Branch> Product_Branches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product_Branch>()
            .HasOne(p => p.Branch) 
            .WithMany(b => b.Product_Branches)
            .HasForeignKey(p => p.IdBranch); 
        }
    }
}
