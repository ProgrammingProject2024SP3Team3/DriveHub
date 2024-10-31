using DriveHubModel;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using DriveHub.Models.Dto;

namespace DriveHubTests
{
    /// <summary>    
    /// User A has cancelled Iron Stallion
    /// </summary>
    public class BookingTests_SET5
    {
        BookingTestFixtures Fixture;

        [Fact]
        public async Task Set5_UserA_Extend_ShouldReturnError()
        {
            Fixture = new BookingTestFixtures(5, "usera");

            // Act
            var result = await Fixture.BookingsController.Extend("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set5_UserB_Extend_ShouldReturnError()
        {
            Fixture = new BookingTestFixtures(5, "userb");

            // Act
            var result = await Fixture.BookingsController.Extend("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set5_UserA_Search_ShouldReturnSearch()
        {
            Fixture = new BookingTestFixtures(5, "usera");

            // Act
            var result = await Fixture.BookingsController.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Search", viewResult.ViewName);
        }

        [Fact]
        public async Task Set5_UserA_Create_ShouldReturnCreate()
        {
            Fixture = new BookingTestFixtures(5, "usera");

            // Act
            var result = await Fixture.BookingsController.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", viewResult.ViewName);
        }

        [Fact]
        public async Task Set5_UserA_CreateConfirmed_ShouldCreate()
        {
            // Arrange
            Fixture = new BookingTestFixtures(5, "usera");

            var reservationDto = new ReservationDto();
            reservationDto.VehicleId = "cac6a77c-59fd-4d0e-b557-9a3230a79e9a";
            reservationDto.StartPodId = "e904170e-a945-4edd-802a-72e214e89cdb";
            reservationDto.QuotedPricePerHour = 20m;

            // Act
            var result = await Fixture.BookingsController.Create(reservationDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Details", viewResult.ViewName);
            var model = Assert.IsAssignableFrom<Booking>(viewResult.Model);
            Assert.Equal("cac6a77c-59fd-4d0e-b557-9a3230a79e9a", model.VehicleId);
        }

        [Fact]
        public async Task Set5_UserA_Cancel_ShouldReturnNotFoundResult()
        {
            // Arrange
            Fixture = new BookingTestFixtures(5, "usera");

            // Act
            var result = await Fixture.BookingsController.Cancel("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set5_UserA_CancelConfirmed_ShouldReturnNotFoundResult()
        {
            // Arrange
            Fixture = new BookingTestFixtures(5, "usera");

            // Act
            var result = await Fixture.BookingsController.CancelConfirmed("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set5_UserA_PrintInvoice_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(5, "usera");

            // Act
            var result = await Fixture.BookingsController.PrintInvoice(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set5_UserA_PrintReport_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(5, "usera");

            // Act
            var result = await Fixture.BookingsController.PrintReport();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set5_UserB_PrintInvoice_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(5, "userb");

            // Act
            var result = await Fixture.BookingsController.PrintInvoice(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set5_UserB_PrintReport_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(5, "userb");

            // Act
            var result = await Fixture.BookingsController.PrintReport();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
