using Microsoft.EntityFrameworkCore;
using PhoneService.DAL.Entities;

namespace PhoneService.DAL.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Detail> Details { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public ApplicationContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

            modelBuilder.Entity<User>()
                        .HasData();

            modelBuilder.Entity<User>()
                        .Ignore(u => u.NewPassword);

            modelBuilder.Entity<User>()
                        .HasMany(u => u.Requests)
                        .WithOne(u => u.Client)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                        .HasMany(u => u.Responses)
                        .WithOne(u => u.Master)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Service>()
                        .HasMany(s => s.Stocks)
                        .WithOne(s => s.Service)
                        .HasForeignKey(s => s.ServiceId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                        .HasOne(r => r.Request)
                        .WithOne(r => r.Review)
                        .HasForeignKey<Review>(r => r.RequestId);

            modelBuilder.Entity<Service>()
                        .HasMany(s => s.Requests)
                        .WithOne(r => r.Service)
                        .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
