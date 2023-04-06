using Microsoft.EntityFrameworkCore;
using Repository.Models.Entities;


namespace Repository
{
    public class ApplicationDbContext : DbContext
    {      
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
                entity.Property(u => u.LastName).HasMaxLength(50).IsRequired();
                entity.Property(u => u.PasswordHash).IsRequired();                

                // Seed data
                entity.HasData(
                    new User
                    {
                        UserId = 1,
                        FirstName = "Admin1",
                        LastName = "User",
                        Email = "admin@gmail.com",
                        PasswordHash = "HBdY9xVn51OsbPsTt6ao4lV/Raq6vkZbMskrFpG69a+njxIAy7xwxWcjXOImDAXsWsjnKxpDrnRVsVttWTLcEA=="
                        // password is "Kurnool*234"
                    }
                );
            });

            modelBuilder.Entity<Role>(entity => 
            {
                entity.HasKey(r => r.RoleId);
                entity.HasIndex(r => r.Name).IsUnique();
                entity.Property(r => r.Name).HasMaxLength(50).IsRequired();
                entity.HasData(
                    new Role
                    {
                        RoleId = 1,
                        Name = "Admin"
                    },
                    new Role
                    {
                        RoleId = 2,
                        Name = "Manager"
                    },
                    new Role
                    {
                        RoleId = 3,
                        Name = "User"
                    }
                );
            });

            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
