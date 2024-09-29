using DriveHubModel;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using NetTopologySuite.Geometries;

namespace DriveHub.Data.SeedData
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
                var vehicleRateDb = new VehicleRate(
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
                var vehicleDb = new Vehicle(
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

            foreach (var site in GetSites(logger))
            {
                var siteDb = new Site(
                    site.SiteName,
                    site.Address,
                    site.City,
                    site.PostCode,
                    site.Latitude,
                    site.Longitude
                    );
                context.Add(siteDb);
            }
            context.SaveChanges();

            foreach (var pod in GetPods(logger))
            {
                if (pod.VehicleId is "") pod.VehicleId = null;
                var podDb = new Pod(
                    pod.PodId,
                    pod.SiteId,
                    pod.VehicleId,
                    pod.PodName
                    );
                context.Add(podDb);
            }
            context.SaveChanges();
        }

        private static IList<DriveHub.SeedData.VehicleRate> GetVehicleRates()
        {
            var vehicleRates = new List<DriveHub.SeedData.VehicleRate>();
            using (var reader = new StreamReader($"Data/SeedData/VehicleRates.csv"))
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

            using (var reader = new StreamReader($"Data/SeedData/Vehicles.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                vehicles = csv.GetRecords<DriveHub.SeedData.Vehicle>().ToList();
            }

            return vehicles;
        }

        private static IList<DriveHub.SeedData.Site> GetSites(ILogger<Program> logger)
        {
            IList<DriveHub.SeedData.Site> sites;
            logger.LogInformation("Loading sites data file.");

            using (var reader = new StreamReader($"Data/SeedData/Sites.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                sites = csv.GetRecords<DriveHub.SeedData.Site>().ToList();
            }

            return sites;
        }

        private static IList<DriveHub.SeedData.Pod> GetPods(ILogger<Program> logger)
        {
            IList<DriveHub.SeedData.Pod> pods;
            logger.LogInformation("Loading pods data file.");

            using (var reader = new StreamReader($"Data/SeedData/Pods.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                pods = csv.GetRecords<DriveHub.SeedData.Pod>().ToList();
            }

            return pods;
        }

        private static IList<ApplicationUser> GetUsers(ILogger<Program> logger)
        {
            // TODO Write get data from csv method
            throw new NotImplementedException();
        }

        private static IList<DriveHub.SeedData.Booking> GetBookings(ILogger<Program> logger)
        {
            // TODO Write get data from csv method
            throw new NotImplementedException();
        }
    }
}
