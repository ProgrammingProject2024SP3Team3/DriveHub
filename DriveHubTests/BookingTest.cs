using DriveHub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DriveHubTests
{
    public class BookingTests
    {
        [Fact]
        public async Task Set1_UserA_Search_ShouldRedirectToCurrent()
        {
            var bookingTestFixtures = new BookingTestFixtures(1, "usera");

            // Act
            var result = await bookingTestFixtures.Controller.Search();

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Set1_UserA_Create_ShouldRedirectToError()
        {
            var bookingTestFixtures = new BookingTestFixtures(1, "usera");

            // Act
            var result = await bookingTestFixtures.Controller.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Set1_UserB_Search_ShouldReturn9Vehicles()
        {
            var bookingTestFixtures = new BookingTestFixtures(1, "userb");

            // Act
            var result = await bookingTestFixtures.Controller.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.Equal(9, model.Pods.Count);
        }

        [Fact]
        public async Task Set1_UserB_Search_CannotFindIronStallion()
        {
            var bookingTestFixtures = new BookingTestFixtures(1, "userb");

            // Act
            var result = await bookingTestFixtures.Controller.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.False(model.Pods.Where(c => c.VehicleId == "cac6a77c-59fd-4d0e-b557-9a3230a79e9a").Any());
        }

        [Fact]
        public async Task Set1_UserB_Create_ShoudReturnCreateView()
        {
            var bookingTestFixtures = new BookingTestFixtures(1, "userb");

            // Act
            var result = await bookingTestFixtures.Controller.Create("eeb7b72c-b362-4513-b84b-baa954c83ce0");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", viewResult.ViewName);
        }

        [Fact]
        public async Task Set1_UserB_Create_IronStallionRedirectsToError()
        {
            var bookingTestFixtures = new BookingTestFixtures(1, "userb");

            // Act
            var result = await bookingTestFixtures.Controller.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set2_UserA_Create_ShouldRedirectToError()
        {
            var bookingTestFixtures = new BookingTestFixtures(2, "usera");

            // Act
            var result = await bookingTestFixtures.Controller.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Set2_UserB_Search_ShouldReturn9Vehicles()
        {
            var bookingTestFixtures = new BookingTestFixtures(2, "userb");

            // Act
            var result = await bookingTestFixtures.Controller.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.Equal(9, model.Pods.Count);
        }

        [Fact]
        public async Task Set2_UserA_Search_ShouldRedirectToCurrent()
        {
            var bookingTestFixtures = new BookingTestFixtures(2, "usera");

            // Act
            var result = await bookingTestFixtures.Controller.Search();

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Set2_UserB_Search_CannotFindIronStallion()
        {
            var bookingTestFixtures = new BookingTestFixtures(2, "userb");

            // Act
            var result = await bookingTestFixtures.Controller.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.False(model.Pods.Where(c => c.VehicleId == "cac6a77c-59fd-4d0e-b557-9a3230a79e9a").Any());
        }

        [Fact]
        public async Task Set2_UserB_Create_IronStallionRedirectsToError()
        {
            var bookingTestFixtures = new BookingTestFixtures(2, "userb");

            // Act
            var result = await bookingTestFixtures.Controller.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }




        //[Fact]
        //public async Task Create_ShouldCreateBooking_WhenValid()
        //{
        //    // Arrange: Set up a mock authenticated user
        //    var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, "dac0b461-0e19-4879-a43c-53be7460f819"), // Ensure this ID matches the seeded user
        //        new Claim(ClaimTypes.Name, "test-user")
        //    }, "mock"));

        //    // Set the mocked user to the controller's context
        //    bookingTestFixtures.SetMockUserToContext(bookingTestFixtures.Controller, mockUser);

        //    // Arrange: Set up valid booking data
        //    var bookingDto = new ReservationDto
        //    {
        //        BookingId = Guid.NewGuid().ToString(),
        //        VehicleId = "5780bc06-a5e2-4598-9eea-7cb90b55e169", // Ensure valid vehicle ID
        //        StartPodId = "77be04ce-fa47-4d5e-81b9-9e8de26230e1", // Ensure valid pod ID
        //        QuotedPricePerHour = 27.50m
        //    };

        //    // Act
        //    var result = await bookingTestFixtures.Controller.Create(bookingDto);

        //    // Assert
        //    var redirectResult = Assert.IsType<ViewResult>(result);
        //    Assert.Equal("Details", redirectResult.ViewName); // Ensure redirect to Details
        //}

        //[Fact]
        //public async Task Create_ShouldFail_WhenStartTimeIsInThePast()
        //{
        //    // Arrange: Set up a mock authenticated user
        //    var mockUser = bookingTestFixtures.CreateMockUser();
        //    bookingTestFixtures.SetMockUserToContext(bookingTestFixtures.Controller, mockUser);

        //    // Arrange: Set up invalid booking data with past StartTime
        //    var bookingDto = new ReservationDto
        //    {
        //        VehicleId = "236d7fac-7e6f-4856-9203-de65bc9e7545",
        //        StartPodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d",
        //        QuotedPricePerHour = 20.50m
        //    };

        //    // Act
        //    var result = await bookingTestFixtures.Controller.Create(bookingDto);

        //    // Assert: Check if ModelState contains errors for StartTime
        //    Assert.IsType<ViewResult>(result);
        //    Assert.False(bookingTestFixtures.Controller.ModelState.IsValid);
        //    Assert.Contains("StartTime", bookingTestFixtures.Controller.ModelState.Keys);
        //}

        //[Fact]
        //public async Task Create_Should_Error_When_User_Enters_Different_PricePerHour()
        //{
        //    // Arrange: Set up a mock authenticated user
        //    var mockUser = bookingTestFixtures.CreateMockUser();
        //    bookingTestFixtures.SetMockUserToContext(bookingTestFixtures.Controller, mockUser);

        //    // Arrange: Set up invalid booking data with past StartTime
        //    var bookingDto = new ReservationDto
        //    {
        //        VehicleId = "236d7fac-7e6f-4856-9203-de65bc9e7545",
        //        StartPodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d",
        //        QuotedPricePerHour = 5
        //    };

        //    // Act
        //    var result = await bookingTestFixtures.Controller.Create(bookingDto);

        //    // Assert
        //    Assert.IsType<RedirectToActionResult>(result);
        //}

        //[Fact]
        //public async Task Create_Should_Error_When_Vehicle_Is_Not_In_Correct_Pod()
        //{
        //    // Arrange: Set up a mock authenticated user
        //    var mockUser = bookingTestFixtures.CreateMockUser();
        //    bookingTestFixtures.SetMockUserToContext(bookingTestFixtures.Controller, mockUser);

        //    // Arrange: Set up illegal booking data with bad pod and vehicle combinations
        //    var bookingDto = new ReservationDto
        //    {
        //        VehicleId = "236d7fac-7e6f-4856-9203-de65bc9e7545",
        //        StartPodId = "5f0244c5-765c-41b5-a4ee-5c01badd5ad6",
        //        QuotedPricePerHour = 20.50m
        //    };

        //    // Act
        //    var result = await bookingTestFixtures.Controller.Create(bookingDto);

        //    // Assert
        //    Assert.IsType<RedirectToActionResult>(result);
        //}

        //[Fact]
        //public async Task Create_Should_Fail_When_Starttime_Is_Before_Endtime()
        //{
        //    // Arrange: Set up a mock authenticated user
        //    var mockUser = bookingTestFixtures.CreateMockUser();
        //    bookingTestFixtures.SetMockUserToContext(bookingTestFixtures.Controller, mockUser);

        //    // Arrange: Set up invalid booking data with past StartTime
        //    var bookingDto = new ReservationDto
        //    {
        //        VehicleId = "236d7fac-7e6f-4856-9203-de65bc9e7545",
        //        StartPodId = "48ef47b8-95f2-42ac-a17d-7fc596dce08d",
        //        QuotedPricePerHour = 20.50m
        //    };

        //    // Act
        //    var result = await bookingTestFixtures.Controller.Create(bookingDto);

        //    // Assert
        //    Assert.False(bookingTestFixtures.Controller.ModelState.IsValid);
        //    Assert.Contains("EndTime", bookingTestFixtures.Controller.ModelState.Keys);
        //    Assert.IsType<ViewResult>(result);
        //}

        //[Fact]
        //public async Task Details_ShouldReturnBooking_WhenExists()
        //{
        //    // Arrange: Ensure the booking exists before calling Details
        //    var bookingId = "4088af8c-08a0-4c26-a629-3a445eef4d26"; // Use an actual existing booking ID from seeded data

        //    // Act
        //    var result = await bookingTestFixtures.Controller.Details(bookingId);

        //    // Assert
        //    if (result is ViewResult viewResult)
        //    {
        //        var model = Assert.IsAssignableFrom<Booking>(viewResult.Model);
        //        Assert.Equal(bookingId, model.BookingId); // Ensure the booking ID matches
        //    }
        //    else if (result is NotFoundResult)
        //    {
        //        Assert.Fail("Booking not found."); // Fail test if booking doesn't exist
        //    }
        //}

        //[Fact]
        //public async Task Details_ShouldReturnNotFound_WhenBookingDoesNotExist()
        //{
        //    // Arrange: Use a non-existing booking ID
        //    var nonExistingBookingId = "non-existing-booking-id";

        //    // Act
        //    var result = await bookingTestFixtures.Controller.Details(nonExistingBookingId);

        //    // Assert
        //    Assert.IsType<NotFoundResult>(result);
        //}

        //// This test simulates a valid booking edit scenario where the user edits the time and vehicle details.
        //[Fact]
        //public async Task Edit_ValidBooking_ShouldReturnUpdatedView()
        //{
        //    // Arrange: Retrieve a valid booking from the database.
        //    var booking = await bookingTestFixtures.Context.Bookings.FirstOrDefaultAsync();
        //    Assert.NotNull(booking); // Ensure that we have at least one booking.

        //    // Prepare the DTO for editing the booking with valid data.
        //    var editBookingDto = new ReservationDto
        //    {
        //        BookingId = booking.BookingId,
        //        VehicleId = booking.VehicleId, // Use existing vehicle ID from booking.
        //        StartPodId = booking.StartPodId, // Use existing start pod ID.
        //        QuotedPricePerHour = booking.PricePerHour // Use the existing price per hour.
        //    };

        //    // Act: Call the Extend method on the controller with the booking details.
        //    var result = await bookingTestFixtures.Controller.Extend(booking.BookingId);

        //    // Assert: Check if the result is a ViewResult and if the booking is updated.
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsType<Booking>(viewResult.Model);
        //    Assert.Equal(booking.BookingId, model.BookingId); // Ensure the returned booking ID matches.
        //}
    }
}
