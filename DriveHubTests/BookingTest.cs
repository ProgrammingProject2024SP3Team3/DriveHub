using Xunit;
using Microsoft.AspNetCore.Mvc;
using DriveHub.Controllers;
using DriveHubModel;
using DriveHub.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DriveHub.Models.ViewModels;
using Moq;
using Microsoft.Extensions.Options;

namespace DriveHubTests
{
    public class BookingTest
    {
        [Fact]
        public async Task Search_ShouldReturnAvailableVehicles()
        {
            // Arrange
            // Set up the options for the in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Create an instance of the in-memory database context
            using (var context = new ApplicationDbContext(options))
            {
                // Seed VehicleRates based on the provided data
                context.VehicleRates.AddRange(
                    new VehicleRate { VehicleRateId = "798e5cce-92bb-493f-915c-f251c0dd674e", Description = "Standard", PricePerHour = 20, EffectiveDate = new DateTime(2024, 9, 21, 15, 39, 51) },
                    new VehicleRate { VehicleRateId = "2452a253-031c-4de5-a6db-03691eac644b", Description = "Electric", PricePerHour = 20.50M, EffectiveDate = new DateTime(2024, 9, 21, 15, 39, 51) },
                    new VehicleRate { VehicleRateId = "f11eec17-116c-4513-abe8-83049c0fa924", Description = "Utility", PricePerHour = 25, EffectiveDate = new DateTime(2024, 9, 21, 15, 39, 51) },
                    new VehicleRate { VehicleRateId = "4f040b3d-f4a9-4b8a-95f7-ed06ea181c34", Description = "SUV", PricePerHour = 27.50M, EffectiveDate = new DateTime(2024, 9, 21, 15, 39, 51) }
                );

                // Seed the in-memory database with vehicle data from the CSV
                context.Vehicles.AddRange(
                    new Vehicle 
                    { 
                        VehicleId = "236d7fac-7e6f-4856-9203-de65bc9e7545", 
                        Make = "Tesla", 
                        Model = "Model Y", 
                        Seats = 5, 
                        Pod = new Pod { PodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d", PodName = "Main Pod" },
                        Colour = "Silver", 
                        Name = "The Silver Bullet", 
                        RegistrationPlate = "SLV001", 
                        State = "VIC", 
                        VehicleRateId = "2452a253-031c-4de5-a6db-03691eac644b",  // Electric rate
                        Year = "2022"
                    },
                    new Vehicle 
                    { 
                        VehicleId = "4e0f6bb8-4835-4960-87ab-b89e21c78ab4", 
                        Make = "Tesla", 
                        Model = "Model Y", 
                        Seats = 5, 
                        Pod = new Pod { PodId = "e9de308d-c76f-4c3e-98b0-a9911fcaa068", PodName = "Secondary Pod" },
                        Colour = "Red", 
                        Name = "Road Warrior", 
                        RegistrationPlate = "WAR001", 
                        State = "VIC", 
                        VehicleRateId = "2452a253-031c-4de5-a6db-03691eac644b",  // Electric rate
                        Year = "2023"
                    }
                    // Add more vehicles as needed
                );

                context.SaveChanges(); // Save the data to the in-memory database

                // Mock ILogger for BookingsController
                var mockLogger = new Mock<ILogger<BookingsController>>();

                // Manually construct UserManager
                var userStore = new Mock<IUserStore<IdentityUser>>().Object;
                var userManager = new UserManager<IdentityUser>(userStore, 
                    new Mock<IOptions<IdentityOptions>>().Object, 
                    new Mock<IPasswordHasher<IdentityUser>>().Object, 
                    new IUserValidator<IdentityUser>[0], 
                    new IPasswordValidator<IdentityUser>[0], 
                    new Mock<ILookupNormalizer>().Object, 
                    new Mock<IdentityErrorDescriber>().Object, 
                    null, 
                    new Mock<ILogger<UserManager<IdentityUser>>>().Object);

                // Instantiate the BookingsController with the in-memory context and mocks
                var controller = new BookingsController(context, mockLogger.Object, userManager);

                // Act
                // Call the Search action of the BookingsController
                var result = await controller.Search();

                // Assert
                // Verify that the result is of type ViewResult
                var viewResult = Assert.IsType<ViewResult>(result);

                // Verify that the model passed to the view is of type BookingSearchVM
                var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.ViewData.Model);

                // Ensure that the model contains the expected number of vehicles
                Assert.Equal(2, model.Vehicles.Count);  // Adjust the count based on your test data
            }
        }
    }
}
