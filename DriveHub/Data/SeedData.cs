using DriveHubModel;
using DriveHub.SeedData;
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
                var vehicleRateDb = new DriveHubModel.VehicleRate(
                    vehicleRate.VehicleRateId,
                    vehicleRate.Description,
                    vehicleRate.PricePerHour,
                    vehicleRate.EffectiveDate
                    );
                context.Add(vehicleRateDb);
            }
            context.SaveChanges();

            foreach (var vehicle in GetVehicles(logger))
            {
                var vehicleDb = new DriveHubModel.Vehicle(
                    vehicle.VehicleId,
                    vehicle.VehicleRateId,
                    vehicle.Make,
                    vehicle.Model,
                    vehicle.RegistrationPlate,
                    vehicle.State,
                    vehicle.Year,
                    vehicle.Seats,
                    vehicle.Colour,
                    vehicle.Name
                    );
                context.Add(vehicleDb);
            }
            context.SaveChanges();

        }

        private static IList<DriveHub.SeedData.VehicleRate> GetVehicleRates()
        {
            var vehicleRates = new List<DriveHub.SeedData.VehicleRate>();
            //vehicleRates.Add(
            //    new VehicleRate("798e5cce-92bb-493f-915c-f251c0dd674e", "Standard", 20, DateTime.Parse("2024-09-21 15:39:51")));
            //vehicleRates.Add(
            //    new VehicleRate("2452a253-031c-4de5-a6db-03691eac644b", "Electric", 20.5m, DateTime.Parse("2024-09-21 15:39:51")));
            //vehicleRates.Add(
            //    new VehicleRate("f11eec17-116c-4513-abe8-83049c0fa924", "Utility", 25, DateTime.Parse("2024-09-21 15:39:51")));
            //vehicleRates.Add(
            //    new VehicleRate("4f040b3d-f4a9-4b8a-95f7-ed06ea181c34", "SUV", 27.5m, DateTime.Parse("2024-09-21 15:39:51")));

            using (var reader = new StreamReader($"Data/VehicleRates.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                vehicleRates = csv.GetRecords<DriveHub.SeedData.VehicleRate>().ToList();
            }

            return vehicleRates;
        }

        private static IList<DriveHub.SeedData.Vehicle> GetVehicles(ILogger<Program> logger)
        {
            IList<DriveHub.SeedData.Vehicle> vehicles;
            logger.LogInformation("Loading vehicles data file.");

            using (var reader = new StreamReader($"Data/Vehicles.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                vehicles = csv.GetRecords<DriveHub.SeedData.Vehicle>().ToList();
            }

            return vehicles;
        }

        private static IList<DriveHub.SeedData.Site> GetSites()
        {
            // TODO Write get data from csv method
            throw new NotImplementedException();
        }

        private static IList<DriveHub.SeedData.Pod> GetPods()
        {
            // TODO Write get data from csv method
            throw new NotImplementedException();
        }

        private static IList<ApplicationUser> GetUsers()
        {
            // TODO Write get data from csv method
            throw new NotImplementedException();
        }

        private static IList<DriveHub.SeedData.Booking> GetBookings()
        {
            // TODO Write get data from csv method
            throw new NotImplementedException();
        }
    }
}
