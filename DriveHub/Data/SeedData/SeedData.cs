using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using DriveHub.Data;

namespace DriveHub.SeedData
{
    public class SeedData
    {
        internal static void Initialize(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            var scope = services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            if (context.VehicleRates.Any()) { return; }

            foreach (var vehicleRate in GetVehicleRates())
            {
                var vehicleRateDb = new DriveHubModel.VehicleRate(
                    vehicleRate.VehicleRateId,
                    vehicleRate.Description,
                    vehicleRate.PricePerHour,
                    vehicleRate.PricePerMinute,
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

            foreach (var site in GetSites(logger))
            {
                var siteDb = new DriveHubModel.Site(
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

            context.Database.ExecuteSqlRaw(@"CREATE SPATIAL INDEX IX_Sites_Location 
                                           ON [Sites]([Location]) 
                                           USING GEOGRAPHY_AUTO_GRID;");

            foreach (var pod in GetPods(logger))
            {
                if (pod.VehicleId is "") pod.VehicleId = null;
                var podDb = new DriveHubModel.Pod(
                    pod.PodId,
                    pod.SiteId,
                    pod.VehicleId,
                    pod.PodName
                    );
                context.Add(podDb);
            }
            context.SaveChanges();
        }

        private static IList<VehicleRate> GetVehicleRates()
        {
            var vehicleRates = new List<VehicleRate>();
            using (var reader = new StreamReader($"Data/SeedData/VehicleRates.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                vehicleRates = csv.GetRecords<VehicleRate>().ToList();
            }
            return vehicleRates;
        }

        private static IList<Vehicle> GetVehicles(ILogger<Program> logger)
        {
            IList<Vehicle> vehicles;
            logger.LogInformation("Loading vehicles data file.");

            using (var reader = new StreamReader($"Data/SeedData/Vehicles.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                vehicles = csv.GetRecords<Vehicle>().ToList();
            }

            return vehicles;
        }

        private static IList<Site> GetSites(ILogger<Program> logger)
        {
            IList<Site> sites;
            logger.LogInformation("Loading sites data file.");

            using (var reader = new StreamReader($"Data/SeedData/Sites.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                sites = csv.GetRecords<Site>().ToList();
            }

            return sites;
        }

        private static IList<Pod> GetPods(ILogger<Program> logger)
        {
            IList<Pod> pods;
            logger.LogInformation("Loading pods data file.");

            using (var reader = new StreamReader($"Data/SeedData/Pods.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                pods = csv.GetRecords<Pod>().ToList();
            }

            return pods;
        }
    }
}
