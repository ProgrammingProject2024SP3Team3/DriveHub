using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Identity;

namespace Admin.Data.SeedData
{
    public class SeedData
    {
        internal static void Initialize(IServiceProvider services)
        {
            var context = services.GetRequiredService<AdminDbContext>();
            context.Database.Migrate();

            var scope = services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            if (context.Users.Any()) { return; }

            foreach (var user in GetUsers(logger))
            {                
                context.Add(user);
            }
            context.SaveChanges();
        }

        private static IList<IdentityUser> GetUsers(ILogger<Program> logger)
        {
            var users = new List<IdentityUser>();
            using (var reader = new StreamReader($"Data/SeedData/Users.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                users = csv.GetRecords<IdentityUser>().ToList();
            }
            return users;
        }
    }
}
