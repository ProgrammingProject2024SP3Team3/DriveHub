using DriveHubModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DriveHub.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Location> Locations { get; set; }
        public DbSet<Pod> Pods { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Location>()
               .HasKey(c => c.LocationID);

            modelBuilder.Entity<Pod>()
               .HasKey(c => c.PodID);

            modelBuilder.Entity<Pod>()
                .HasOne(c => c.Location);

            modelBuilder.Entity<Vehicle>()
               .HasKey(c => c.VehicleID);
        }
    }
}