using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DriveHubTests
{
    public class VehicleTests_SET5
    {
        VehiclesTestFixtures Fixture;

        [Fact]
        public async Task Set5_UserA_Pickup_ShouldReturnCreate()
        {
            // Arrange
            Fixture = new VehiclesTestFixtures(5, "usera");

            // Act
            var result = await Fixture.VehiclesController.Pickup("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Create", redirectToActionResult.ActionName);

            Assert.NotNull(redirectToActionResult.RouteValues);
            Assert.True(redirectToActionResult.RouteValues.ContainsKey("id"));
            Assert.Equal("cac6a77c-59fd-4d0e-b557-9a3230a79e9a", redirectToActionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task Set5_UserA_Dropoff_ShouldReturnError()
        {
            // Arrange
            Fixture = new VehiclesTestFixtures(5, "usera");

            // Act
            var result = await Fixture.VehiclesController.Dropoff("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set5_UserB_Pickup_ShouldReturnCreate()
        {
            // Arrange
            Fixture = new VehiclesTestFixtures(5, "userb");

            // Act
            var result = await Fixture.VehiclesController.Pickup("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Create", redirectToActionResult.ActionName);

            Assert.NotNull(redirectToActionResult.RouteValues);
            Assert.True(redirectToActionResult.RouteValues.ContainsKey("id"));
            Assert.Equal("cac6a77c-59fd-4d0e-b557-9a3230a79e9a", redirectToActionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task Set5_UserB_Dropoff_ShouldReturnError()
        {
            // Arrange
            Fixture = new VehiclesTestFixtures(5, "userb");

            // Act
            var result = await Fixture.VehiclesController.Dropoff("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }
    }
}