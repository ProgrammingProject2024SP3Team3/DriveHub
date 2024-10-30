﻿using Microsoft.AspNetCore.Identity;
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
    public class VehiclesTestFixtures
    {
        public ApplicationDbContext Context { get; private set; }
        public VehiclesController VehiclesController { get; private set; }
        public Mock<ILogger<VehiclesController>> VehiclesLogger { get; private set; }
        public UserManager<IdentityUser> UserManager { get; private set; }

        private User UserA = new User("0e4ed3e4-bc98-44c4-acc0-e7aa647ae703", "usera@test.com");

        private User UserB = new User("4cb2fe87-451a-4a35-baa9-da24ca377cc9", "userb@test.com");

        public VehiclesTestFixtures(int set, string userName)
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            Context = new ApplicationDbContext(options);
            VehiclesLogger = new Mock<ILogger<VehiclesController>>();

            // Seed the database with test data using TestDataBuilder
            TestDataBuilder.SeedData(Context, set);

            // Mock the UserManager and set up the controller
            UserManager = MockUserManager();
            VehiclesController = new VehiclesController(Context, VehiclesLogger.Object, UserManager);

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

            VehiclesController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockUser }
            };
        }

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
