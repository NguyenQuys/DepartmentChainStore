using Microsoft.EntityFrameworkCore;

namespace UserService_5002.Models
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserOtherInfo> UserOtherInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserOtherInfo)
                .WithOne(w => w.User)
                .HasForeignKey<UserOtherInfo>(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<UserOtherInfo>()
                .HasOne(uo => uo.Role)
                .WithMany()
                .HasForeignKey(ou=>ou.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
