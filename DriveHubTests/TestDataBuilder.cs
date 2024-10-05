using DriveHubModel;
using DriveHub.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Globalization;
using System.IO;

public static class TestDataBuilder
{
    public static void SeedData(ApplicationDbContext context)
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory; // Base directory of the test run
        string testDataPath = Path.Combine(basePath, "TestData");

        string ratesCsvPath = Path.Combine(testDataPath, "VehicleRates.csv");
        string vehiclesCsvPath = Path.Combine(testDataPath, "Vehicles.csv");
        string podsCsvPath = Path.Combine(testDataPath, "Pods.csv");
        string sitesCsvPath = Path.Combine(testDataPath, "Sites.csv");
        string usersCsvPath = Path.Combine(testDataPath, "Users.csv"); // For seeding users
        string bookingsCsvPath = Path.Combine(testDataPath, "Bookings.csv"); // For seeding bookings

        // Seed Users from CSV
        using (var reader = new StreamReader(usersCsvPath))
        {
            bool skipHeader = true;  // To skip the first row (header)
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (skipHeader)
                {
                    skipHeader = false;
                    continue;
                }

                var values = line.Split(',');

                if (values.Length < 15) continue; // Ensure there are enough values

                var user = new IdentityUser
                {
                    Id = values[0],
                    UserName = values[1],
                    NormalizedUserName = values[2],
                    Email = values[3],
                    NormalizedEmail = values[4],
                    EmailConfirmed = ParseBoolean(values[5]), // Handle incorrect formats
                    PasswordHash = values[6],
                    SecurityStamp = values[7],
                    ConcurrencyStamp = values[8],
                    PhoneNumber = values[9],
                    PhoneNumberConfirmed = ParseBoolean(values[10]), // Handle incorrect formats
                    TwoFactorEnabled = ParseBoolean(values[11]), // Handle incorrect formats
                    LockoutEnd = ParseDateTime(values[12]), // Handle '0' or empty strings
                    LockoutEnabled = ParseBoolean(values[13]), // Handle incorrect formats
                    AccessFailedCount = int.Parse(values[14])
                };

                if (!context.Users.Any(u => u.Id == user.Id))
                {
                    context.Users.Add(user);
                }
            }
        }

        // Seed VehicleRates from CSV
        using (var reader = new StreamReader(ratesCsvPath))
        {
            bool skipHeader = true;  
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (skipHeader)
                {
                    skipHeader = false;
                    continue;
                }

                var values = line.Split(',');
                if (values.Length < 4) continue;

                if (!context.VehicleRates.Any(vr => vr.VehicleRateId == values[0]))
                {
                    context.VehicleRates.Add(new VehicleRate
                    {
                        VehicleRateId = values[0],
                        Description = values[1],
                        PricePerHour = decimal.Parse(values[2], CultureInfo.InvariantCulture),
                        EffectiveDate = DateTime.Parse(values[3], CultureInfo.InvariantCulture)
                    });
                }
            }
        }

        // Seed Vehicles from CSV
        using (var reader = new StreamReader(vehiclesCsvPath))
        {
            bool skipHeader = true;  
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (skipHeader)
                {
                    skipHeader = false;
                    continue;
                }

                var values = line.Split(',');
                if (values.Length < 10) continue;

                if (!context.Vehicles.Any(v => v.VehicleId == values[0]))
                {
                    context.Vehicles.Add(new Vehicle
                    {
                        VehicleId = values[0],
                        VehicleRateId = values[1],
                        Make = values[2],
                        Model = values[3],
                        RegistrationPlate = values[4],
                        State = values[5],
                        Year = values[6],
                        Seats = int.Parse(values[7]),
                        Colour = values[8],
                        Name = values[9]
                    });
                }
            }
        }

        // Seed Pods from CSV
        using (var reader = new StreamReader(podsCsvPath))
        {
            bool skipHeader = true;  
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (skipHeader)
                {
                    skipHeader = false;
                    continue;
                }

                var values = line.Split(',');
                if (values.Length < 4) continue;

                if (!context.Pods.Any(p => p.PodId == values[0]))
                {
                    context.Pods.Add(new Pod
                    {
                        PodId = values[0],
                        SiteId = int.Parse(values[1]),
                        VehicleId = values[2],
                        PodName = values[3]
                    });
                }
            }
        }

        // Seed Sites from CSV
        using (var reader = new StreamReader(sitesCsvPath))
        {
            bool skipHeader = true;  
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (skipHeader)
                {
                    skipHeader = false;
                    continue;
                }

                var values = line.Split(',');
                if (values.Length < 7) continue;

                if (!context.Sites.Any(s => s.SiteId == int.Parse(values[0])))
                {
                    context.Sites.Add(new Site
                    {
                        SiteId = int.Parse(values[0]),
                        SiteName = values[1],
                        Address = values[2],
                        City = values[3],
                        PostCode = values[4],
                        Latitude = double.Parse(values[5], CultureInfo.InvariantCulture),
                        Longitude = double.Parse(values[6], CultureInfo.InvariantCulture),
                        Location = CreateGeographyPoint(values[5], values[6])  // Handle Geography point creation
                    });
                }
            }
        }

        // Seed Bookings from CSV
        using (var reader = new StreamReader(bookingsCsvPath))
        {
            bool skipHeader = true;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (skipHeader)
                {
                    skipHeader = false;
                    continue;
                }

                var values = line.Split(',');
                if (values.Length < 8) continue;

                if (!context.Bookings.Any(b => b.BookingId == values[0]))
                {
                    context.Bookings.Add(new Booking
                    {
                        BookingId = values[0],
                        VehicleId = values[1],
                        Id = values[2], // User ID from AspNetUsers
                        StartPodId = values[3],
                        EndPodId = values[4],
                        StartTime = DateTime.Parse(values[5], CultureInfo.InvariantCulture),
                        EndTime = DateTime.Parse(values[6], CultureInfo.InvariantCulture),
                        PricePerHour = decimal.Parse(values[7], CultureInfo.InvariantCulture),
                        BookingStatus = (BookingStatus)Enum.Parse(typeof(BookingStatus), values[8])
                    });
                }
            }
        }

        context.SaveChanges();
    }

    private static Point CreateGeographyPoint(string latitude, string longitude)
    {
        var coordinate = new Coordinate(double.Parse(latitude, CultureInfo.InvariantCulture), double.Parse(longitude, CultureInfo.InvariantCulture));
        return new Point(coordinate) { SRID = 4326 };  // Use correct SRID for geographic data
    }

    // Helper method to handle '0' or empty DateTime fields
    private static DateTime? ParseDateTime(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value == "0")
        {
            return null;
        }

        return DateTime.Parse(value, CultureInfo.InvariantCulture);
    }

    private static bool ParseBoolean(string value)
    {
        return value.Equals("true", StringComparison.OrdinalIgnoreCase) || value == "1";
    }
}
