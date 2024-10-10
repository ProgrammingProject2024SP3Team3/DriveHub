using DriveHub.Models.Dto;
using DriveHub.Models.ViewModels;
using DriveHubModel;
using System.Security.Claims;
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
        public async Task Search_ShouldReturnEmptyList_WhenNoVehiclesAreAvailable()
        {
            // Arrange: Remove all vehicles from the context
            bookingTestFixtures.Context.Vehicles.RemoveRange(bookingTestFixtures.Context.Vehicles);
            await bookingTestFixtures.Context.SaveChangesAsync();

            // Act
            var result = await bookingTestFixtures.Controller.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.ViewData.Model);
            Assert.Empty(model.Vehicles); // Ensure the vehicle list is empty
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
            bookingTestFixtures.SetMockUserToContext(bookingTestFixtures.Controller, mockUser);

            // Arrange: Set up valid booking data
            var bookingDto = new BookingDto
            {
                VehicleId = "5780bc06-a5e2-4598-9eea-7cb90b55e169", // Ensure valid vehicle ID
                StartPodId = "77be04ce-fa47-4d5e-81b9-9e8de26230e1", // Ensure valid pod ID
                EndPodId = "77be04ce-fa47-4d5e-81b9-9e8de26230e1", // Ensure valid pod ID
                StartTime = DateTime.Now.AddHours(1), // Ensure time within validation
                EndTime = DateTime.Now.AddHours(2),
                QuotedPricePerHour = 27.50m
            };

            // Act
            var result = await bookingTestFixtures.Controller.Create(bookingDto);

            // Assert
            var redirectResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Details", redirectResult.ViewName); // Ensure redirect to Details
        }

        [Fact]
        public async Task Create_ShouldFail_WhenStartTimeIsInThePast()
        {
            // Arrange: Set up a mock authenticated user
            var mockUser = bookingTestFixtures.CreateMockUser();
            bookingTestFixtures.SetMockUserToContext(bookingTestFixtures.Controller, mockUser);

            // Arrange: Set up invalid booking data with past StartTime
            var bookingDto = new BookingDto
            {
                VehicleId = "236d7fac-7e6f-4856-9203-de65bc9e7545",
                StartPodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d",
                EndPodId = "4121be84-af99-4be1-80b9-5c4fd2117567",
                StartTime = DateTime.Now.AddHours(-1), // Start time in the past
                EndTime = DateTime.Now.AddHours(1),
                QuotedPricePerHour = 20.50m
            };

            // Act
            var result = await bookingTestFixtures.Controller.Create(bookingDto);

            // Assert: Check if ModelState contains errors for StartTime
            Assert.IsType<ViewResult>(result);
            Assert.False(bookingTestFixtures.Controller.ModelState.IsValid);
            Assert.Contains("StartTime", bookingTestFixtures.Controller.ModelState.Keys);
        }

        [Fact]
        public async Task Create_Should_Fail_When_Booking_Time_Is_Less_Than_30_mins()
        {
            // Arrange: Set up a mock authenticated user
            var mockUser = bookingTestFixtures.CreateMockUser();
            bookingTestFixtures.SetMockUserToContext(bookingTestFixtures.Controller, mockUser);

            // Arrange: Set up invalid booking data with past StartTime
            var bookingDto = new BookingDto
            {
                VehicleId = "236d7fac-7e6f-4856-9203-de65bc9e7545",
                StartPodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d",
                EndPodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d",
                StartTime = DateTime.Now.AddMinutes(10),
                EndTime = DateTime.Now.AddMinutes(15),
                QuotedPricePerHour = 20.50m
            };

            // Act
            var result = await bookingTestFixtures.Controller.Create(bookingDto);

            Assert.False(bookingTestFixtures.Controller.ModelState.IsValid);
            Assert.IsType<ViewResult>(result);
            Assert.Contains("EndTime", bookingTestFixtures.Controller.ModelState.Keys);
        }

        [Fact]
        public async Task Create_Should_Error_When_User_Enters_Different_PricePerHour()
        {
            // Arrange: Set up a mock authenticated user
            var mockUser = bookingTestFixtures.CreateMockUser();
            bookingTestFixtures.SetMockUserToContext(bookingTestFixtures.Controller, mockUser);

            // Arrange: Set up invalid booking data with past StartTime
            var bookingDto = new BookingDto
            {
                VehicleId = "236d7fac-7e6f-4856-9203-de65bc9e7545",
                StartPodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d",
                EndPodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d",
                StartTime = DateTime.Now.AddMinutes(10),
                EndTime = DateTime.Now.AddMinutes(45),
                QuotedPricePerHour = 5
            };

            // Act
            var result = await bookingTestFixtures.Controller.Create(bookingDto);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Create_Should_Error_When_Vehicle_Is_Not_In_Correct_Pod()
        {
            // Arrange: Set up a mock authenticated user
            var mockUser = bookingTestFixtures.CreateMockUser();
            bookingTestFixtures.SetMockUserToContext(bookingTestFixtures.Controller, mockUser);

            // Arrange: Set up illegal booking data with bad pod and vehicle combinations
            var bookingDto = new BookingDto
            {
                VehicleId = "236d7fac-7e6f-4856-9203-de65bc9e7545",
                StartPodId = "5f0244c5-765c-41b5-a4ee-5c01badd5ad6",
                EndPodId = "5f0244c5-765c-41b5-a4ee-5c01badd5ad6",
                StartTime = DateTime.Now.AddMinutes(10),
                EndTime = DateTime.Now.AddMinutes(15),
                QuotedPricePerHour = 20.50m
            };

            // Act
            var result = await bookingTestFixtures.Controller.Create(bookingDto);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Create_Should_Fail_When_Starttime_Is_Before_Endtime()
        {
            // Arrange: Set up a mock authenticated user
            var mockUser = bookingTestFixtures.CreateMockUser();
            bookingTestFixtures.SetMockUserToContext(bookingTestFixtures.Controller, mockUser);

            // Arrange: Set up invalid booking data with past StartTime
            var bookingDto = new BookingDto
            {
                VehicleId = "236d7fac-7e6f-4856-9203-de65bc9e7545",
                StartPodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d",
                EndPodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d",
                StartTime = DateTime.Now.AddMinutes(60),
                EndTime = DateTime.Now.AddMinutes(10),
                QuotedPricePerHour = 20.50m
            };

            // Act
            var result = await bookingTestFixtures.Controller.Create(bookingDto);

            // Assert
            Assert.False(bookingTestFixtures.Controller.ModelState.IsValid);
            Assert.Contains("EndTime", bookingTestFixtures.Controller.ModelState.Keys);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Should_Fail_When_Starttime_Is_Greater_Than_7_days()
        {
            // Arrange: Set up a mock authenticated user
            var mockUser = bookingTestFixtures.CreateMockUser();
            bookingTestFixtures.SetMockUserToContext(bookingTestFixtures.Controller, mockUser);

            // Arrange: Set up invalid booking data with past StartTime
            var bookingDto = new BookingDto
            {
                VehicleId = "236d7fac-7e6f-4856-9203-de65bc9e7545",
                StartPodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d",
                EndPodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d",
                StartTime = DateTime.Now.AddDays(7).AddMinutes(30),
                EndTime = DateTime.Now.AddDays(8),
                QuotedPricePerHour = 20.50m
            };

            // Act
            var result = await bookingTestFixtures.Controller.Create(bookingDto);

            // Assert
            Assert.False(bookingTestFixtures.Controller.ModelState.IsValid);
            Assert.Contains("StartTime", bookingTestFixtures.Controller.ModelState.Keys);
            Assert.IsType<ViewResult>(result);
        }

        // NOTE: This test conflicts with the M3 booking logic. We might need to implement this logic in M4.
        // M3 Fact: A vehicle in a pod is available for booking.
        // M3 Fact: If a vehicle has-a booking, then it does not have-a pod.
        //[Fact]
        //public async Task Create_ShouldFail_WhenBookingConflictsWithExistingBooking()
        //{
        //    // Arrange: Seed a conflicting booking
        //    bookingTestFixtures.Context.Bookings.Add(new Booking
        //    {
        //        BookingId = Guid.NewGuid().ToString(),
        //        VehicleId = "236d7fac-7e6f-4856-9203-de65bc9e7545",
        //        StartPodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d",
        //        EndPodId = "e9de308d-c76f-4c3e-98b0-a9911fcaa068",
        //        StartTime = DateTime.Now.AddHours(1),
        //        EndTime = DateTime.Now.AddHours(3),
        //        PricePerHour = 20,
        //        BookingStatus = BookingStatus.InProgress,
        //        Id = "test-user-id" // Ensure this matches the expected user ID
        //    });
        //    await bookingTestFixtures.Context.SaveChangesAsync();

        //    // Arrange: Attempt to create a booking with conflicting times
        //    var bookingDto = new BookingDto
        //    {
        //        VehicleId = "236d7fac-7e6f-4856-9203-de65bc9e7545",
        //        StartPodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d",
        //        EndPodId = "e9de308d-c76f-4c3e-98b0-a9911fcaa068",
        //        StartTime = DateTime.Now.AddHours(2), // Overlaps with existing booking
        //        EndTime = DateTime.Now.AddHours(4),
        //        QuotedPricePerHour = 20
        //    };

        //    // Act
        //    var result = await bookingTestFixtures.Controller.Create(bookingDto);

        //    // Assert: Check for conflict in ModelState
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.False(bookingTestFixtures.Controller.ModelState.IsValid);
        //    Assert.Contains("VehicleId", bookingTestFixtures.Controller.ModelState.Keys);
        //}

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

        [Fact]
        public async Task Details_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            // Arrange: Use a non-existing booking ID
            var nonExistingBookingId = "non-existing-booking-id";

            // Act
            var result = await bookingTestFixtures.Controller.Details(nonExistingBookingId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // This test simulates a valid booking edit scenario where the user edits the time and vehicle details.
        [Fact]
        public async Task Edit_ValidBooking_ShouldReturnUpdatedView()
        {
            // Arrange: Retrieve a valid booking ID from the seeded data
            var booking = await bookingTestFixtures.Context.Bookings.FirstOrDefaultAsync();
            Assert.NotNull(booking); // Ensure that we have at least one booking

            var editBookingDto = new EditBookingDto
            {
                BookingId = booking.BookingId,
                VehicleId = booking.VehicleId, // Use existing vehicle ID from booking
                StartPodId = booking.StartPodId, // Use existing start pod ID
                EndPodId = booking.EndPodId, // Use existing end pod ID
                StartTime = DateTime.Now.AddHours(1), 
                EndTime = DateTime.Now.AddHours(2),
                QuotedPricePerHour = booking.PricePerHour // Use the existing price per hour
            };

            // Act: Call the Edit method on the controller with the booking details
            var result = await bookingTestFixtures.Controller.Edit(booking.BookingId, editBookingDto);

            // Assert: Check if the result is a ViewResult and the booking is updated
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Booking>(viewResult.Model);
            Assert.Equal(booking.BookingId, model.BookingId);
        }


        [Fact]
        public async Task Edit_ShouldFail_WhenStartTimeIsInThePast()
        {
            // Arrange: Retrieve a valid booking ID from the seeded data
            var booking = await bookingTestFixtures.Context.Bookings.FirstOrDefaultAsync();
            Assert.NotNull(booking); // Ensure that we have at least one booking

            var editBookingDto = new EditBookingDto
            {
                BookingId = booking.BookingId,
                VehicleId = booking.VehicleId,
                StartPodId = booking.StartPodId,
                EndPodId = booking.EndPodId,
                StartTime = DateTime.Now.AddHours(-1), // Set StartTime in the past
                EndTime = DateTime.Now.AddHours(1),
                QuotedPricePerHour = booking.PricePerHour
            };

            // Act: Call the Edit method
            var result = await bookingTestFixtures.Controller.Edit(booking.BookingId, editBookingDto);

            // Assert: Ensure the model state is invalid
            Assert.IsType<ViewResult>(result);
            Assert.False(bookingTestFixtures.Controller.ModelState.IsValid);
            Assert.Contains("StartTime", bookingTestFixtures.Controller.ModelState.Keys);
        }
        
        [Fact]
        public async Task Edit_ShouldFail_WhenEndTimeIsBeforeStartTime()
        {
            // Arrange: Retrieve a valid booking ID from the seeded data
            var booking = await bookingTestFixtures.Context.Bookings.FirstOrDefaultAsync();
            Assert.NotNull(booking); // Ensure that we have at least one booking

            var editBookingDto = new EditBookingDto
            {
                BookingId = booking.BookingId,
                VehicleId = booking.VehicleId,
                StartPodId = booking.StartPodId,
                EndPodId = booking.EndPodId,
                StartTime = DateTime.Now.AddHours(2), // StartTime is after EndTime
                EndTime = DateTime.Now.AddHours(1),
                QuotedPricePerHour = booking.PricePerHour
            };

            // Act: Call the Edit method
            var result = await bookingTestFixtures.Controller.Edit(booking.BookingId, editBookingDto);

            // Assert: Ensure the model state is invalid
            Assert.IsType<ViewResult>(result);
            Assert.False(bookingTestFixtures.Controller.ModelState.IsValid);
            Assert.Contains("EndTime", bookingTestFixtures.Controller.ModelState.Keys);
        }

        [Fact]
        public async Task Edit_ShouldFail_WhenBookingTimeLessThan30Minutes()
        {
            // Arrange: Retrieve a valid booking ID from the seeded data
            var booking = await bookingTestFixtures.Context.Bookings.FirstOrDefaultAsync();
            Assert.NotNull(booking); // Ensure that we have at least one booking

            var editBookingDto = new EditBookingDto
            {
                BookingId = booking.BookingId,
                VehicleId = booking.VehicleId,
                StartPodId = booking.StartPodId,
                EndPodId = booking.EndPodId,
                StartTime = DateTime.Now.AddMinutes(10),
                EndTime = DateTime.Now.AddMinutes(20), // Booking time is less than 30 minutes
                QuotedPricePerHour = booking.PricePerHour
            };

            // Act: Call the Edit method
            var result = await bookingTestFixtures.Controller.Edit(booking.BookingId, editBookingDto);

            // Assert: Ensure the model state is invalid
            Assert.IsType<ViewResult>(result);
            Assert.False(bookingTestFixtures.Controller.ModelState.IsValid);
            Assert.Contains("EndTime", bookingTestFixtures.Controller.ModelState.Keys);
        }

        [Fact]
        public async Task Edit_Should_Fail_When_Booking_Time_Is_Greater_Than_7_Days()
        {
            // Arrange: Set up a mock authenticated user
            var mockUser = bookingTestFixtures.CreateMockUser();
            bookingTestFixtures.SetMockUserToContext(bookingTestFixtures.Controller, mockUser);

            // Arrange: Retrieve an existing booking
            var booking = await bookingTestFixtures.Context.Bookings.FirstOrDefaultAsync();
            Assert.NotNull(booking); // Ensure that we have a valid booking

            var editBookingDto = new EditBookingDto
            {
                BookingId = booking.BookingId,
                VehicleId = booking.VehicleId,
                StartPodId = booking.StartPodId,
                EndPodId = booking.EndPodId,
                StartTime = DateTime.Now.AddDays(8), // Start time is more than 7 days in advance
                EndTime = DateTime.Now.AddDays(8).AddHours(1),
                QuotedPricePerHour = booking.PricePerHour
            };

            // Act: Call the Edit method
            var result = await bookingTestFixtures.Controller.Edit(booking.BookingId, editBookingDto);

            // Assert: Ensure the model state is invalid
            Assert.False(bookingTestFixtures.Controller.ModelState.IsValid);
            Assert.Contains("StartTime", bookingTestFixtures.Controller.ModelState.Keys);
            Assert.IsType<ViewResult>(result);
        }

    }
}
