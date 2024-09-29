using DriveHubModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Admin.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Site> Sites { get; set; }
        public DbSet<Pod> Pods { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleRate> VehicleRates { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Journey> Journeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define keys
            modelBuilder.Entity<Site>()
               .HasKey(c => c.SiteId);

            modelBuilder.Entity<Pod>()
               .HasKey(c => c.PodId);

            modelBuilder.Entity<Vehicle>()
               .HasKey(c => c.VehicleId);

            modelBuilder.Entity<VehicleRate>()
                .HasKey(c => c.VehicleRateId);

            // Set "geography" column type for spatial data in the Site entity
            //modelBuilder.Entity<Site>()
            //   .Property(l => l.Location)
            //   .HasColumnType("geography");

            // Define relationships
            modelBuilder.Entity<Site>()
                .HasMany(c => c.Pods)
                .WithOne(c => c.Site)
                .HasForeignKey(c => c.SiteId);

            modelBuilder.Entity<Pod>()
                .HasOne(c => c.Site)
                .WithMany(c => c.Pods)
                .HasForeignKey(c => c.SiteId);

            modelBuilder.Entity<Vehicle>()
                .HasOne(c => c.VehicleRate)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(c => c.VehicleRateId);

            modelBuilder.Entity<VehicleRate>()
                .HasMany(c => c.Vehicles)
                .WithOne(c => c.VehicleRate);

            modelBuilder.Entity<VehicleRate>()
                .Property(c => c.PricePerHour)
                .HasColumnType("Money");

            modelBuilder.Entity<Booking>()
                .HasOne(c => c.ApplicationUser)
                .WithMany(c => c.Bookings)
                .HasForeignKey(c => c.Id);

            modelBuilder.Entity<Booking>()
                .HasOne(c => c.Vehicle)
                .WithMany(c => c.Bookings)
                .HasForeignKey(c => c.VehicleId);

            modelBuilder.Entity<Booking>()
                .Property(c => c.PricePerHour)
                .HasColumnType("Money");

            modelBuilder
                .Entity<Booking>()
                .Property(e => e.BookingStatus)
                .HasConversion(
                    v => v.ToString(),
                    v => (BookingStatus)Enum.Parse(typeof(BookingStatus), v));

            modelBuilder.Entity<Journey>()
                .HasOne(c => c.Booking)
                .WithOne(c => c.Journey);

            modelBuilder.Entity<Journey>()
                .Property(c => c.Price)
                .HasColumnType("Money");
        }
    }
}
