using DriveHubModel;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using DriveHub.Models.Dto;

namespace DriveHubTests
{
    /// <summary>
    /// User A has extended reservation for Iron Stallion
    /// </summary>
    public class BookingTests_SET7
    {
        BookingTestFixtures Fixture;

        [Fact]
        public async Task Set7_UserA_Extend_ShouldReturnError()
        {
            Fixture = new BookingTestFixtures(7, "usera");

            // Act
            var result = await Fixture.BookingsController.Extend("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set7_UserB_Extend_ShouldReturnError()
        {
            Fixture = new BookingTestFixtures(7, "userb");

            // Act
            var result = await Fixture.BookingsController.Extend("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set7_UserA_Search_ShouldRedirectToCurrent()
        {
            Fixture = new BookingTestFixtures(7, "usera");

            // Act
            var result = await Fixture.BookingsController.Search();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Current", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set7_UserA_Create_ShouldRedirectToCurrent()
        {
            Fixture = new BookingTestFixtures(7, "usera");

            // Act
            var result = await Fixture.BookingsController.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Current", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set7_UserA_CreateConfirmed_ShouldRedirectToCurrent()
        {
            // Arrange
            Fixture = new BookingTestFixtures(7, "usera");

            var reservationDto = new DriveHub.Views.Bookings.Create();
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
        public async Task Set7_UserA_Cancel_ShouldReturnCancelViewForIronStallion()
        {
            // Arrange
            Fixture = new BookingTestFixtures(7, "usera");

            // Act
            var result = await Fixture.BookingsController.Cancel("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Booking>(viewResult.Model);
            Assert.Equal("cac6a77c-59fd-4d0e-b557-9a3230a79e9a", model.VehicleId);
        }

        [Fact]
        public async Task Set7_UserA_CancelConfirmed_ShouldCancelBooking()
        {
            // Arrange
            Fixture = new BookingTestFixtures(7, "usera");

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
        public async Task Set7_UserA_PrintInvoice_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(7, "usera");

            // Act
            var result = await Fixture.BookingsController.PrintInvoice(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set7_UserA_PrintReport_ShouldReturnFile()
        {
            Fixture = new BookingTestFixtures(7, "usera");

            // Act
            var result = await Fixture.BookingsController.PrintReport();

            // Assert
            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async Task Set7_UserB_PrintInvoice_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(7, "userb");

            // Act
            var result = await Fixture.BookingsController.PrintInvoice(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set7_UserB_PrintReport_ShouldReturnFile()
        {
            Fixture = new BookingTestFixtures(7, "userb");

            // Act
            var result = await Fixture.BookingsController.PrintReport();

            // Assert
            Assert.IsType<FileContentResult>(result);
        }
    }
}
