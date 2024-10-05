using DriveHub.Controllers;
using DriveHub.Models.Dto;
using DriveHub.Models.ViewModels;
using DriveHubModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace DriveHubTests
{
    public class BookingTests
    {
        private readonly BookingTestFixtures bookingTestFixtures;

        public BookingTests()
        {
            bookingTestFixtures = new BookingTestFixtures();
        }

        [Fact]
        public async Task Search_ShouldReturnAvailableVehicles()
        {
            // Act
            var result = await bookingTestFixtures.Controller.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.ViewData.Model);
            Assert.NotEmpty(model.Vehicles); // Ensure that there are vehicles available
        }

        [Fact]
        public async Task Create_ShouldCreateBooking_WhenValid()
        {
            // Arrange: Set up a mock authenticated user
            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "dac0b461-0e19-4879-a43c-53be7460f819"), // Ensure this ID matches the seeded user
                new Claim(ClaimTypes.Name, "test-user")
            }, "mock"));

            // Set the mocked user to the controller's context
            bookingTestFixtures.Controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = mockUser }
            };

            // Arrange: Set up valid booking data
            var bookingDto = new BookingDto
            {
                VehicleId = "236d7fac-7e6f-4856-9203-de65bc9e7545", // Ensure valid vehicle ID
                StartPodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d", // Ensure valid pod ID
                EndPodId = "e9de308d-c76f-4c3e-98b0-a9911fcaa068", // Ensure valid pod ID
                StartTime = DateTime.Now.AddHours(1), // Ensure time within validation
                EndTime = DateTime.Now.AddHours(2),
                QuotedPricePerHour = 20
            };

            // Act
            var result = await bookingTestFixtures.Controller.Create(bookingDto);

            // Assert: Check ModelState for validity
            if (!bookingTestFixtures.Controller.ModelState.IsValid)
            {
                var errors = bookingTestFixtures.Controller.ViewData.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .Select(e => new
                    {
                        Field = e.Key,
                        Errors = e.Value.Errors.Select(err => err.ErrorMessage).ToArray()
                    });

                // Log each field with its associated validation errors
                foreach (var error in errors)
                {
                    Console.WriteLine($"Field: {error.Field}, Errors: {string.Join(", ", error.Errors)}");
                }

                Assert.Fail("ModelState should be valid, but it was invalid. See the logged errors for more details.");
            }

            // Assert: Ensure that the correct ViewResult is returned
            var viewResult = Assert.IsType<ViewResult>(result);
            var booking = Assert.IsType<Booking>(viewResult.Model);

            // Assert: Validate that the booking was saved correctly in the database
            var createdBooking = await bookingTestFixtures.Context.Bookings.FirstOrDefaultAsync(b => b.VehicleId == bookingDto.VehicleId);
            Assert.NotNull(createdBooking);
            Assert.Equal(bookingDto.VehicleId, createdBooking.VehicleId);
            Assert.Equal(bookingDto.StartPodId, createdBooking.StartPodId);
            Assert.Equal(bookingDto.EndPodId, createdBooking.EndPodId);
            Assert.Equal(bookingDto.StartTime, createdBooking.StartTime);
            Assert.Equal(bookingDto.EndTime, createdBooking.EndTime);
        }

        [Fact]
        public async Task Details_ShouldReturnBooking_WhenExists()
        {
            // Arrange: Ensure the booking exists before calling Details
            var bookingId = "4088af8c-08a0-4c26-a629-3a445eef4d26"; // Use an actual existing booking ID from seeded data

            // Act
            var result = await bookingTestFixtures.Controller.Details(bookingId);

            // Assert
            if (result is ViewResult viewResult)
            {
                var model = Assert.IsAssignableFrom<Booking>(viewResult.Model);
                Assert.Equal(bookingId, model.BookingId); // Ensure the booking ID matches
            }
            else if (result is NotFoundResult)
            {
                Assert.Fail("Booking not found."); // Fail test if booking doesn't exist
            }
        }
    }
}
