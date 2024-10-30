using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DriveHub.Data;
using DriveHub.Controllers;
using Microsoft.Extensions.Configuration;

namespace DriveHubTests
{
    public class BookingTestFixtures : IDisposable
    {
        public ApplicationDbContext Context { get; private set; }
        public BookingsController BookingsController { get; private set; }
        public Mock<ILogger<BookingsController>> BookingsLogger { get; private set; }
        public UserManager<IdentityUser> UserManager { get; private set; }

        public IConfiguration Configuration { get; private set; }

        private User UserA = new User("0e4ed3e4-bc98-44c4-acc0-e7aa647ae703", "usera@test.com");

        private User UserB = new User("4cb2fe87-451a-4a35-baa9-da24ca377cc9", "userb@test.com");

        public BookingTestFixtures(int set, string userName)
        {
            // Use a unique database name for each test to ensure isolation
            var uniqueDbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(uniqueDbName)
                .Options;

            Context = new ApplicationDbContext(options);
            BookingsLogger = new Mock<ILogger<BookingsController>>();

            // Seed the database with test data using TestDataBuilder
            TestDataBuilder.SeedData(Context, set);

            // Mock the UserManager and set up the controller
            UserManager = MockUserManager();
            BookingsController = new BookingsController(Context, BookingsLogger.Object, UserManager, Configuration);

            // Set up a default mock authenticated user for tests
            SetMockAuthenticatedUser(userName);
        }

        // Public method to allow other test classes to reuse the mocked UserManager
        public UserManager<IdentityUser> MockUserManager()
        {
            var userStore = new Mock<IUserStore<IdentityUser>>().Object;
            return new UserManager<IdentityUser>(userStore, new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<IdentityUser>>().Object, Array.Empty<IUserValidator<IdentityUser>>(),
                Array.Empty<IPasswordValidator<IdentityUser>>(), new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object, null,
                new Mock<ILogger<UserManager<IdentityUser>>>().Object);
        }

        // Method to mock an authenticated user for tests that require it
        private void SetMockAuthenticatedUser(string userName)
        {
            // Set the user for the test
            User? user = null;
            if (userName == "usera") { user = UserA; }
            else if (userName == "userb") { user = UserB; }
            if (user == null) throw new Exception();

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id), // Ensure this matches a user seeded in Users.csv
                new Claim(ClaimTypes.Name, user.UserName)
            }, "mock"));

            BookingsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockUser }
            };
        }

        public void Dispose()
        {
            Context.Dispose();
            BookingsController = null;
            BookingsLogger = null;
            UserManager = null;
        }
    }
}
