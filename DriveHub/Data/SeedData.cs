using DriveHubModel;
using Microsoft.EntityFrameworkCore;

namespace DriveHub.Data
{
    public class SeedData
    {
        internal static void Initialize(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            var scope = services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            if (context.Vehicles.Any()) { return; }

            // TODO Write seed data method
        }

        private static ICollection<Vehicle> GetVehicles()
        {
            // TODO Write get data from csv method
            throw new NotImplementedException();
        }

        private static ICollection<VehicleRate> GetVehicleRates()
        {
            // TODO Write get data from csv method
            throw new NotImplementedException();
        }

        private static ICollection<Site> GetSites()
        {
            // TODO Write get data from csv method
            throw new NotImplementedException();
        }

        private static ICollection<Pod> GetPods()
        {
            // TODO Write get data from csv method
            throw new NotImplementedException();
        }

        private static ICollection<ApplicationUser> GetUsers()
        {
            // TODO Write get data from csv method
            throw new NotImplementedException();
        }
    }
}
