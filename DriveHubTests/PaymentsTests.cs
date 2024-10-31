using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DriveHubTests
{
    public class PaymentsTests
    {
        PaymentsTestFixtures Fixture;

        // Set 3 Tests

        [Fact]
        public async Task Set3_UserA_Success_ShouldBeSuccess()
        {
            Fixture = new PaymentsTestFixtures(3, "usera");

            // Act
            var result = await Fixture.PaymentsController.Success("3cab88d0-603a-4bc6-a0cb-9cff6de2d86b");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set3_UserA_Cancel_ShouldReturnDetails()
        {
            Fixture = new PaymentsTestFixtures(3, "usera");

            // Act
            var result = await Fixture.PaymentsController.Cancel("3cab88d0-603a-4bc6-a0cb-9cff6de2d86b");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set3_UserB_Success_ShouldReturnError()
        {
            Fixture = new PaymentsTestFixtures(3, "userb");

            // Act
            var result = await Fixture.PaymentsController.Success("3cab88d0-603a-4bc6-a0cb-9cff6de2d86b");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set3_UserB_Cancel_ShouldReturnError()
        {
            Fixture = new PaymentsTestFixtures(3, "userb");

            // Act
            var result = await Fixture.PaymentsController.Success("3cab88d0-603a-4bc6-a0cb-9cff6de2d86b");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
        }
    }
}
