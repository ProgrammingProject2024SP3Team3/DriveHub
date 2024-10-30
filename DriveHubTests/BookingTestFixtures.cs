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
    internal class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        public User(string id, string userName)
        {
            Id = id;
            UserName = userName;
        }
    }

    public class BookingTestFixtures
    {
        public ApplicationDbContext Context { get; private set; }
        public BookingsController Controller { get; private set; }
        public Mock<ILogger<BookingsController>> MockLogger { get; private set; }
        public UserManager<IdentityUser> UserManager { get; private set; }

        public IConfiguration Configuration { get; private set; }

        private User UserA = new User("0e4ed3e4-bc98-44c4-acc0-e7aa647ae703", "usera@test.com");

        private User UserB = new User("4cb2fe87-451a-4a35-baa9-da24ca377cc9", "userb@test.com");

        public BookingTestFixtures(int set, string userName)
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            Context = new ApplicationDbContext(options);
            MockLogger = new Mock<ILogger<BookingsController>>();

            // Seed the database with test data using TestDataBuilder
            TestDataBuilder.SeedData(Context, set);

            // Mock the UserManager and set up the controller
            UserManager = MockUserManager();
            Controller = new BookingsController(Context, MockLogger.Object, UserManager, Configuration);

            // Set up a default mock authenticated user for tests
            SetMockAuthenticatedUser(userName);
        }

        // Public method to allow other test classes to reuse the mocked UserManager
        public UserManager<IdentityUser> MockUserManager()
        {
            var userStore = new Mock<IUserStore<IdentityUser>>().Object;
            return new UserManager<IdentityUser>(userStore, new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<IdentityUser>>().Object, new IUserValidator<IdentityUser>[0],
                new IPasswordValidator<IdentityUser>[0], new Mock<ILookupNormalizer>().Object,
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

            Controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockUser }
            };
        }

        //// Public method to create mock users for specific cases
        //public ClaimsPrincipal CreateMockUser(string userId = "dac0b461-0e19-4879-a43c-53be7460f819", string userName = "test-user")
        //{
        //    return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, userId),
        //        new Claim(ClaimTypes.Name, userName)
        //    }, "mock"));
        //}

        // Public method to set a mock user in the controller for a specific test case
        public void SetMockUserToContext(Controller controller, ClaimsPrincipal mockUser)
        {
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = mockUser }
            };
        }
    }
}
