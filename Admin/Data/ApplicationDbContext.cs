using DriveHubModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Admin.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Site> Sites { get; set; }
        public DbSet<Pod> Pods { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleRate> VehicleRates { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define keys
            modelBuilder.Entity<VehicleRate>()
                .HasKey(c => c.VehicleRateId);

            modelBuilder.Entity<Vehicle>()
               .HasKey(c => c.VehicleId);

            modelBuilder.Entity<Site>()
               .HasKey(c => c.SiteId);

            modelBuilder.Entity<Pod>()
               .HasKey(c => c.PodId);

            modelBuilder.Entity<VehicleRate>()
                .HasMany(c => c.Vehicles)
                .WithOne(c => c.VehicleRate);

            modelBuilder.Entity<VehicleRate>()
                .Property(c => c.PricePerHour)
                .HasColumnType("Money");

            modelBuilder.Entity<VehicleRate>()
                .Property(c => c.PricePerMinute)
                .HasColumnType("Money");

            modelBuilder.Entity<Vehicle>()
                .HasOne(c => c.VehicleRate)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(c => c.VehicleRateId);

            modelBuilder.Entity<Vehicle>()
                .HasOne(c => c.Pod)
                .WithOne(c => c.Vehicle)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // Define relationships
            modelBuilder.Entity<Site>()
                .HasMany(c => c.Pods)
                .WithOne(c => c.Site);

            modelBuilder.Entity<Pod>()
                .HasOne(c => c.Site)
                .WithMany(c => c.Pods)
                .HasForeignKey(c => c.SiteId);

            modelBuilder.Entity<Pod>()
                .HasOne(c => c.Vehicle)
                .WithOne(c => c.Pod)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Booking>()
                .HasOne(c => c.ApplicationUser)
                .WithMany(c => c.Bookings)
                .HasForeignKey(c => c.Id);

            modelBuilder.Entity<Booking>()
                .HasOne(c => c.Vehicle)
                .WithMany(c => c.Bookings)
                .HasForeignKey(c => c.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Booking>()
                .HasOne(c => c.StartPod)
                .WithMany(c => c.StartPods)
                .HasForeignKey(c => c.StartPodId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Booking>()
                .HasOne(c => c.EndPod)
                .WithMany(c => c.EndPods)
                .HasForeignKey(c => c.EndPodId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Booking>()
                .Property(c => c.PricePerHour)
                .HasColumnType("Money");

            modelBuilder.Entity<Booking>()
                .Property(c => c.PricePerMinute)
                .HasColumnType("Money");

            modelBuilder
                .Entity<Booking>()
                .Property(e => e.BookingStatus)
                .HasConversion(
                    v => v.ToString(),
                    v => (BookingStatus)Enum.Parse(typeof(BookingStatus), v));

            modelBuilder
                .Entity<Booking>()
                .HasOne(c => c.Invoice)
                .WithOne(c => c.Booking);

            modelBuilder
                .Entity<Booking>()
                .HasOne(c => c.Receipt)
                .WithOne(c => c.Booking);

            modelBuilder.Entity<Invoice>()
                .HasOne(c => c.Booking)
                .WithOne(c => c.Invoice);

            modelBuilder.Entity<Invoice>()
                .Property(c => c.Amount)
                .HasColumnType("Money");

            modelBuilder.Entity<Receipt>()
                .HasOne(c => c.Booking)
                .WithOne(c => c.Receipt);

            modelBuilder.Entity<Receipt>()
                .Property(c => c.Amount)
                .HasColumnType("Money");

        }
    }
}
