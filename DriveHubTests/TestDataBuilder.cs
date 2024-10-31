using DriveHubModel;
using DriveHub.Data;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using System.Globalization;
public static class TestDataBuilder
{
    public static void SeedData(ApplicationDbContext context, int set)
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory; // Base directory of the test run
        string testDataPath = Path.Combine(basePath, "TestData");

        string ratesCsvPath = Path.Combine(testDataPath, "VehicleRates.csv");
        string vehiclesCsvPath = Path.Combine(testDataPath, $"Vehicles_SET{set}.csv");
        string invoicesCsvPath = Path.Combine(testDataPath, $"Invoices_SET{set}.csv");
        string podsCsvPath = Path.Combine(testDataPath, $"Pods_SET{set}.csv");
        string sitesCsvPath = Path.Combine(testDataPath, "Sites.csv");
        string usersCsvPath = Path.Combine(testDataPath, "Users.csv"); // For seeding users
        string bookingsCsvPath = Path.Combine(testDataPath, $"Bookings_SET{set}.csv"); // For seeding bookings

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
                        PricePerMinute = decimal.Parse(values[3], CultureInfo.InvariantCulture),
                        EffectiveDate = DateTime.Parse(values[4], CultureInfo.InvariantCulture)
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
                    var vehicle = new Vehicle();
                    vehicle.VehicleId = values[0];
                    vehicle.VehicleRateId = values[1];
                    vehicle.IsReserved = Convert.ToBoolean(Convert.ToInt32(values[2]));
                    vehicle.Make = values[3];
                    vehicle.Model = values[4];
                    vehicle.RegistrationPlate = values[5];
                    vehicle.State = values[6];
                    vehicle.Year = values[7];
                    vehicle.Seats = int.Parse(values[8]);
                    vehicle.Colour = values[9];
                    vehicle.Name = values[10];
                    context.Vehicles.Add(vehicle);
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
                    var booking = new Booking();
                    booking.BookingId = values[0];
                    booking.Expires = DateTime.Parse(values[1], CultureInfo.InvariantCulture);
                    booking.IsExtended = Convert.ToBoolean(Convert.ToInt32(values[2]));
                    booking.PaymentId = values[3];
                    booking.VehicleId = values[4];
                    booking.Id = values[5]; // User ID from AspNetUsers
                    booking.StartPodId = values[6];
                    if ("NULL" == values[7]) booking.EndPodId = null;
                    else booking.EndPodId = values[7];
                    if ("NULL" == values[8]) booking.StartTime = null;
                    else booking.StartTime = DateTime.Parse(values[8], CultureInfo.InvariantCulture);
                    if ("NULL" == values[9]) booking.EndTime = null;
                    else booking.EndTime = DateTime.Parse(values[9], CultureInfo.InvariantCulture);
                    booking.PricePerHour = decimal.Parse(values[10], CultureInfo.InvariantCulture);
                    booking.PricePerMinute = decimal.Parse(values[11], CultureInfo.InvariantCulture);
                    booking.BookingStatus = (BookingStatus)Enum.Parse(typeof(BookingStatus), values[12]);

                    context.Bookings.Add(booking);
                }
            }
        }

        // Seed Sites from CSV
        using (var reader = new StreamReader(invoicesCsvPath))
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
                //if (values.Length < 7) continue;

                if (!context.Invoices.Any(s => s.InvoiceNumber == int.Parse(values[0])))
                {
                    Invoice invoice = new Invoice();
                    invoice.InvoiceNumber = int.Parse(values[0]);
                    invoice.BookingId = values[1];
                    invoice.DateTime = DateTime.Parse(values[2], CultureInfo.InvariantCulture);
                    invoice.Amount = decimal.Parse(values[3]);
                    context.Invoices.Add(invoice);
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
