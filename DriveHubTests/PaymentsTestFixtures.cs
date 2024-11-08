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
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using DriveHubModel;

namespace DriveHubTests
{
    public class PaymentsTestFixtures : IDisposable
    {
        public ApplicationDbContext Context { get; private set; }
        public PaymentsController PaymentsController { get; private set; }
        public Mock<ILogger<PaymentsController>> PaymentsLogger { get; private set; }
        public UserManager<ApplicationUser> UserManager { get; private set; }

        public IConfiguration Configuration { get; private set; }

        private User UserA = new User("0e4ed3e4-bc98-44c4-acc0-e7aa647ae703", "usera@test.com");

        private User UserB = new User("4cb2fe87-451a-4a35-baa9-da24ca377cc9", "userb@test.com");

        public PaymentsTestFixtures(int set, string userName)
        {
            // Use a unique database name for each test to ensure isolation
            var uniqueDbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(uniqueDbName)
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            Context = new ApplicationDbContext(options);
            PaymentsLogger = new Mock<ILogger<PaymentsController>>();

            // Seed the database with test data using TestDataBuilder
            TestDataBuilder.SeedData(Context, set);

            // Mock the UserManager and set up the controller
            UserManager = MockUserManager();
            PaymentsController = new PaymentsController(Context, PaymentsLogger.Object, UserManager, Configuration);

            // Set up a default mock authenticated user for tests
            SetMockAuthenticatedUser(userName);
        }

        // Public method to allow other test classes to reuse the mocked UserManager
        public UserManager<ApplicationUser> MockUserManager()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>().Object;
            return new UserManager<ApplicationUser>(userStore, new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<ApplicationUser>>().Object, Array.Empty<IUserValidator<ApplicationUser>>(),
                Array.Empty<IPasswordValidator<ApplicationUser>>(), new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object, null,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object);
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

            PaymentsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockUser }
            };
        }
        public void Dispose()
        {
            Context.Dispose();
            PaymentsController = null;
            PaymentsLogger = null;
            UserManager = null;
        }
    }
}
