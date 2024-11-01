using DriveHub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DriveHubTests
{
    /// <summary>    
    /// User A has unpaid Iron Stallion
    /// </summary>
    public class BookingTests_SET3
    {
        BookingTestFixtures Fixture;

        [Fact]
        public async Task Set3_UserA_Extend_ShouldReturnError()
        {
            Fixture = new BookingTestFixtures(3, "usera");

            // Act
            var result = await Fixture.BookingsController.Extend("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set3_UserA_Create_ShouldRedirectToCurrent()
        {
            Fixture = new BookingTestFixtures(3, "usera");

            // Act
            var result = await Fixture.BookingsController.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Current", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set3_UserB_Search_ShouldReturn10Vehicles()
        {
            Fixture = new BookingTestFixtures(3, "userb");

            // Act
            var result = await Fixture.BookingsController.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.Equal(10, model.Pods.Count);
        }

        [Fact]
        public async Task Set3_UserA_Search_ShouldRedirectToCurrent()
        {
            Fixture = new BookingTestFixtures(3, "usera");

            // Act
            var result = await Fixture.BookingsController.Search();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Current", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set3_UserB_Search_CanFindIronStallion()
        {
            Fixture = new BookingTestFixtures(3, "userb");

            // Act
            var result = await Fixture.BookingsController.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.True(model.Pods.Where(c => c.VehicleId == "cac6a77c-59fd-4d0e-b557-9a3230a79e9a").Any());
        }

        [Fact]
        public async Task Set3_UserB_Create_IronStallionShouldReturnCreateView()
        {
            Fixture = new BookingTestFixtures(3, "userb");

            // Act
            var result = await Fixture.BookingsController.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", viewResult.ViewName);
        }

        [Fact]
        public async Task Set3_UserA_PrintInvoice_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(3, "usera");

            // Act
            var result = await Fixture.BookingsController.PrintInvoice(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set3_UserA_PrintReport_ShouldReturnFile()
        {
            Fixture = new BookingTestFixtures(4, "usera");

            // Act
            var result = await Fixture.BookingsController.PrintReport();

            // Assert
            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async Task Set3_UserB_PrintInvoice_ShouldReturnNotFound()
        {
            Fixture = new BookingTestFixtures(3, "userb");

            // Act
            var result = await Fixture.BookingsController.PrintInvoice(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Set3_UserB_PrintReport_ShouldReturnFile()
        {
            Fixture = new BookingTestFixtures(3, "userb");

            // Act
            var result = await Fixture.BookingsController.PrintReport();

            // Assert
            Assert.IsType<FileContentResult>(result);
        }
    }
}
