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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();

                // Seed data
                entity.HasData(
                    new User
                    {
                        Id = 1,
                        FirstName = "Admin1",
                        LastName = "User",
                        Email = "admin@gmail.com",
                        PasswordHash = "HBdY9xVn51OsbPsTt6ao4lV/Raq6vkZbMskrFpG69a+njxIAy7xwxWcjXOImDAXsWsjnKxpDrnRVsVttWTLcEA=="
                        // password is "Kurnool*234"
                    }
                );
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
