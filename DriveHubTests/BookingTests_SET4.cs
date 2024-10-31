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
