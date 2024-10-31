using DriveHubModel;
using DriveHub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using DriveHub.Models.Dto;

namespace DriveHubTests
{
    /// <summary>
    /// User A has a reservation for Iron Stallion
    /// </summary>
    public class BookingTests_SET1
    {
        BookingTestFixtures Fixture;

        [Fact]
        public async Task Set1_UserA_Extend_ShouldExtendReservation()
        {
            Fixture = new BookingTestFixtures(1, "usera");

            // Act
            var result = await Fixture.BookingsController.Extend("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Current", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set1_UserA_Search_ShouldRedirectToCurrent()
        {
            Fixture = new BookingTestFixtures(1, "usera");

            // Act
            var result = await Fixture.BookingsController.Search();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Current", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set1_UserA_Create_ShouldRedirectToCurrent()
        {
            Fixture = new BookingTestFixtures(1, "usera");

            // Act
            var result = await Fixture.BookingsController.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Current", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set1_UserA_CreateConfirmed_ShouldRedirectToCurrent()
        {
            // Arrange
            Fixture = new BookingTestFixtures(1, "usera");

            var reservationDto = new ReservationDto();
            reservationDto.VehicleId = "cac6a77c-59fd-4d0e-b557-9a3230a79e9a";
            reservationDto.StartPodId = "e904170e-a945-4edd-802a-72e214e89cdb";
            reservationDto.QuotedPricePerHour = 20m;

            // Act
            var result = await Fixture.BookingsController.Create(reservationDto);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Current", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set1_UserA_Cancel_ShouldReturnCancelViewForIronStallion()
        {
            // Arrange
            Fixture = new BookingTestFixtures(1, "usera");

            // Act
            var result = await Fixture.BookingsController.Cancel("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Booking>(viewResult.Model);
            Assert.Equal("cac6a77c-59fd-4d0e-b557-9a3230a79e9a", model.VehicleId);
        }

        [Fact]
        public async Task Set1_UserA_CancelConfirmed_ShouldCancelBooking()
        {
            // Arrange
            Fixture = new BookingTestFixtures(1, "usera");

            // Act
            var result = await Fixture.BookingsController.CancelConfirmed("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectToActionResult.ActionName);
            Assert.Equal("Bookings", redirectToActionResult.ControllerName);

            // Verify that the booking is indeed cancelled
            var booking = await Fixture.Context.Bookings.FindAsync("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");
            Assert.NotNull(booking);
            Assert.Equal(BookingStatus.Cancelled, booking.BookingStatus);
        }

        [Fact]
        public async Task Set1_UserB_Search_ShouldReturn9Vehicles()
        {
            Fixture = new BookingTestFixtures(1, "userb");

            // Act
            var result = await Fixture.BookingsController.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.Equal(9, model.Pods.Count);
        }

        [Fact]
        public async Task Set1_UserB_Search_CannotFindIronStallion()
        {
            Fixture = new BookingTestFixtures(1, "userb");

            // Act
            var result = await Fixture.BookingsController.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.False(model.Pods.Where(c => c.VehicleId == "cac6a77c-59fd-4d0e-b557-9a3230a79e9a").Any());
        }

        [Fact]
        public async Task Set1_UserB_Create_ShouldReturnCreateView()
        {
            Fixture = new BookingTestFixtures(1, "userb");

            // Act
            var result = await Fixture.BookingsController.Create("eeb7b72c-b362-4513-b84b-baa954c83ce0");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", viewResult.ViewName);
        }

        [Fact]
        public async Task Set1_UserB_Create_IronStallionReturnsErrorView()
        {
            Fixture = new BookingTestFixtures(1, "userb");

            // Act
            var result = await Fixture.BookingsController.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set1_UserB_CreateConfirmed_IronStallionReturnsErrorView()
        {
            // Arrange
            Fixture = new BookingTestFixtures(1, "userb");

            var reservationDto = new ReservationDto();
            reservationDto.VehicleId = "cac6a77c-59fd-4d0e-b557-9a3230a79e9a";
            reservationDto.StartPodId = "454f2073-c06d-4dfa-a976-d8f1d2cc93d2";
            reservationDto.QuotedPricePerHour = 27.50m;

            // Act
            var result = await Fixture.BookingsController.Create(reservationDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set1_UserB_CreateConfirmed_LightningMcSpeedShouldReturnDetails()
        {
            // Arrange
            Fixture = new BookingTestFixtures(1, "userb");

            var reservationDto = new ReservationDto();
            reservationDto.VehicleId = "eeb7b72c-b362-4513-b84b-baa954c83ce0";
            reservationDto.StartPodId = "454f2073-c06d-4dfa-a976-d8f1d2cc93d2";
            reservationDto.QuotedPricePerHour = 27.50m;

            // Act
            var result = await Fixture.BookingsController.Create(reservationDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Details", viewResult.ViewName);
        }

        [Fact]
        public async Task Set1_UserB_Extend_ShouldReturnError()
        {
            Fixture = new BookingTestFixtures(1, "userb");

            // Act
            var result = await Fixture.BookingsController.Extend("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set1_UserA_PrintInvoice_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(1, "usera");

            // Act
            var result = await Fixture.BookingsController.PrintInvoice(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set1_UserA_PrintReport_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(1, "usera");

            // Act
            var result = await Fixture.BookingsController.PrintReport();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set1_UserB_PrintInvoice_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(4, "userb");

            // Act
            var result = await Fixture.BookingsController.PrintInvoice(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set1_UserB_PrintReport_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(1, "userb");

            // Act
            var result = await Fixture.BookingsController.PrintReport();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}