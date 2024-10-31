using DriveHubModel;
using DriveHub.Models.ViewModels;
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
