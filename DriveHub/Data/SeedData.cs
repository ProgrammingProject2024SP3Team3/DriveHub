using DriveHubModel;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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

            if (context.Users.Any()) { return; }

            foreach (var vehicleRate in GetVehicleRates())
            {
                context.Add(vehicleRate);
            }
            context.SaveChanges();

            foreach (var vehicle in GetVehicles())
            {
                context.Add(vehicle);
            }
            context.SaveChanges();
        }

        private static IList<VehicleRate> GetVehicleRates()
        {
            IList<VehicleRate> vehicleRates;
            using (var reader = new StreamReader($"{Directory.GetCurrentDirectory()}/Data/VehicleRates.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                vehicleRates = csv.GetRecords<VehicleRate>().ToList();
                reader.Close();
            }

            return vehicleRates;
        }

        private static IList<Vehicle> GetVehicles()
        {
            IList<Vehicle> vehicles;
            using (var reader = new StreamReader($@"{Directory.GetCurrentDirectory()}/Data/Vehicles.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                vehicles = csv.GetRecords<Vehicle>().ToList();
                reader.Close();
            }

            return vehicles ?? throw new AggregateException("Unable to contact web service");
        }

        private static IList<Site> GetSites()
        {
            // TODO Write get data from csv method
            throw new NotImplementedException();
        }

        private static IList<Pod> GetPods()
        {
            // TODO Write get data from csv method
            throw new NotImplementedException();
        }

        private static IList<ApplicationUser> GetUsers()
        {
            // TODO Write get data from csv method
            throw new NotImplementedException();
        }

        private static IList<Booking> GetBookings()
        {
            // TODO Write get data from csv method
            throw new NotImplementedException();
        }
    }
}
