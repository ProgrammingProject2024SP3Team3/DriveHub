using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DriveHubTests
{
    public class PaymentsTests_SET7
    {
        PaymentsTestFixtures Fixture;

        [Fact]
        public async Task Set7_UserA_Success_ShouldReturnError()
        {
            Fixture = new PaymentsTestFixtures(7, "usera");

            // Act
            var result = await Fixture.PaymentsController.Success("3cab88d0-603a-4bc6-a0cb-9cff6de2d86b");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set7_UserA_Cancel_ShouldReturnError()
        {
            Fixture = new PaymentsTestFixtures(7, "usera");

            // Act
            var result = await Fixture.PaymentsController.Cancel("3cab88d0-603a-4bc6-a0cb-9cff6de2d86b");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set7_UserB_Success_ShouldReturnError()
        {
            Fixture = new PaymentsTestFixtures(7, "userb");

            // Act
            var result = await Fixture.PaymentsController.Success("3cab88d0-603a-4bc6-a0cb-9cff6de2d86b");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set7_UserB_Cancel_ShouldReturnError()
        {
            Fixture = new PaymentsTestFixtures(7, "userb");

            // Act
            var result = await Fixture.PaymentsController.Cancel("3cab88d0-603a-4bc6-a0cb-9cff6de2d86b");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
        }
    }
}
