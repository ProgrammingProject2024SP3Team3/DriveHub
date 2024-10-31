using DriveHub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using DriveHub.Models.Dto;

namespace DriveHubTests
{
    /// <summary>    
    /// User A has collected Iron Stallion
    /// </summary>
    public class BookingTests_SET2
    {
        BookingTestFixtures Fixture;

        [Fact]
        public async Task Set2_UserA_Extend_ShouldReturnError()
        {
            Fixture = new BookingTestFixtures(2, "usera");

            // Act
            var result = await Fixture.BookingsController.Extend("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set2_UserA_Create_ShouldRedirectToCurrent()
        {
            Fixture = new BookingTestFixtures(2, "usera");

            // Act
            var result = await Fixture.BookingsController.Create("36d4bc1e-6159-46df-ad98-da93a38bcc82");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Current", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set2_UserA_CreateConfirmed_RedirectsToCurrent()
        {
            // Arrange
            Fixture = new BookingTestFixtures(2, "usera");

            var reservationDto = new ReservationDto();
            reservationDto.VehicleId = "36d4bc1e-6159-46df-ad98-da93a38bcc82";
            reservationDto.StartPodId = "e904170e-a945-4edd-802a-72e214e89cdb";
            reservationDto.QuotedPricePerHour = 20m;

            // Act
            var result = await Fixture.BookingsController.Create(reservationDto);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Current", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set2_UserB_Search_ShouldReturn9Vehicles()
        {
            Fixture = new BookingTestFixtures(2, "userb");

            // Act
            var result = await Fixture.BookingsController.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.Equal(9, model.Pods.Count);
        }

        [Fact]
        public async Task Set2_UserA_Search_ShouldRedirectToCurrent()
        {
            Fixture = new BookingTestFixtures(2, "usera");

            // Act
            var result = await Fixture.BookingsController.Search();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Current", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set2_UserB_Search_CannotFindIronStallion()
        {
            Fixture = new BookingTestFixtures(2, "userb");

            // Act
            var result = await Fixture.BookingsController.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.False(model.Pods.Where(c => c.VehicleId == "cac6a77c-59fd-4d0e-b557-9a3230a79e9a").Any());
        }

        [Fact]
        public async Task Set2_UserB_Create_IronStallionRedirectsToError()
        {
            Fixture = new BookingTestFixtures(2, "userb");

            // Act
            var result = await Fixture.BookingsController.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set2_UserB_CreateConfirmed_IronStallionRedirectsToError()
        {
            // Arrange
            Fixture = new BookingTestFixtures(2, "userb");

            var reservationDto = new ReservationDto();
            reservationDto.VehicleId = "cac6a77c-59fd-4d0e-b557-9a3230a79e9a";
            reservationDto.StartPodId = "e904170e-a945-4edd-802a-72e214e89cdb";
            reservationDto.QuotedPricePerHour = 20m;

            // Act
            var result = await Fixture.BookingsController.Create(reservationDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set2_UserA_PrintInvoice_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(2, "usera");

            // Act
            var result = await Fixture.BookingsController.PrintInvoice(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set2_UserA_PrintReport_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(2, "usera");

            // Act
            var result = await Fixture.BookingsController.PrintReport();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set2_UserB_PrintInvoice_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(2, "userb");

            // Act
            var result = await Fixture.BookingsController.PrintInvoice(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set2_UserB_PrintReport_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(2, "userb");

            // Act
            var result = await Fixture.BookingsController.PrintReport();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
