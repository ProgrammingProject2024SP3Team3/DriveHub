using DriveHub.Models.Dto;
using DriveHub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DriveHubTests
{
    /// <summary>    
    /// User A has complete Iron Stallion
    /// </summary>
    public class BookingTests_SET4
    {
        BookingTestFixtures Fixture;

        [Fact]
        public async Task Set4_UserA_Extend_ShouldReturnError()
        {
            Fixture = new BookingTestFixtures(4, "usera");

            // Act
            var result = await Fixture.BookingsController.Extend("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set4_UserA_Create_ShouldReturnCreate()
        {
            Fixture = new BookingTestFixtures(4, "usera");

            // Act
            var result = await Fixture.BookingsController.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", viewResult.ViewName);
        }

        [Fact]
        public async Task Set4_UserB_Search_ShouldReturn10Vehicles()
        {
            Fixture = new BookingTestFixtures(4, "userb");

            // Act
            var result = await Fixture.BookingsController.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.Equal(10, model.Pods.Count);
        }

        [Fact]
        public async Task Set4_UserA_Search_ShouldReturnSearch()
        {
            Fixture = new BookingTestFixtures(4, "usera");

            // Act
            var result = await Fixture.BookingsController.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Search", viewResult.ViewName);
        }

        [Fact]
        public async Task Set4_UserB_Search_CanFindIronStallion()
        {
            Fixture = new BookingTestFixtures(4, "userb");

            // Act
            var result = await Fixture.BookingsController.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.True(model.Pods.Where(c => c.VehicleId == "cac6a77c-59fd-4d0e-b557-9a3230a79e9a").Any());
        }

        [Fact]
        public async Task Set4_UserB_Create_IronStallionReturnsCreate()
        {
            Fixture = new BookingTestFixtures(4, "userb");

            // Act
            var result = await Fixture.BookingsController.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", viewResult.ViewName);
        }

        [Fact]
        public async Task Set4_UserB_CreateConfirmed_IronStallionRedirectsToError()
        {
            // Arrange
            Fixture = new BookingTestFixtures(4, "userb");

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
        public async Task Set4_UserA_PrintInvoice_ShouldReturnFile()
        {
            Fixture = new BookingTestFixtures(4, "usera");

            // Act
            var result = await Fixture.BookingsController.PrintInvoice(1);

            // Assert
            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async Task Set4_UserA_PrintReport_ShouldReturnFile()
        {
            Fixture = new BookingTestFixtures(4, "usera");

            // Act
            var result = await Fixture.BookingsController.PrintReport();

            // Assert
            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async Task Set4_UserB_PrintInvoice_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(4, "userb");

            // Act
            var result = await Fixture.BookingsController.PrintInvoice(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set4_UserB_PrintReport_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(4, "userb");

            // Act
            var result = await Fixture.BookingsController.PrintReport();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
