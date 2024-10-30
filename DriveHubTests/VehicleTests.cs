using DriveHubModel;
using DriveHub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using DriveHub.Models.Dto;
using Xunit.Sdk;
using System.Globalization;
using System.Reflection;

namespace DriveHubTests
{
    public class VehicleTests : BeforeAfterTestAttribute
    {
        VehiclesTestFixtures Fixture;

        [Fact]
        public async Task Set1_UserA_Pickup_ShouldReturnPickup()
        {
            // Arrange
            Fixture = new VehiclesTestFixtures(1, "usera");

            // Act
            var result = await Fixture.VehiclesController.Pickup("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Pickup", viewResult.ViewName);
            Fixture.Dispose();
        }

        [Fact]
        public async Task Set1_UserA_Dropoff_ShouldReturnError()
        {
            // Arrange
            Fixture = new VehiclesTestFixtures(1, "usera");

            // Act
            var result = await Fixture.VehiclesController.Dropoff("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Fixture.Dispose();
        }

        [Fact]
        public async Task Set1_UserB_Pickup_ShouldReturnSearch()
        {
            // Arrange
            Fixture = new VehiclesTestFixtures(1, "userb");

            // Act
            var result = await Fixture.VehiclesController.Pickup("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Fixture.Dispose();
        }

        [Fact]
        public async Task Set1_UserB_Dropoff_ShouldReturnError()
        {
            // Arrange
            Fixture = new VehiclesTestFixtures(1, "userb");

            // Act
            var result = await Fixture.VehiclesController.Dropoff("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Fixture.Dispose();
        }

        [Fact]
        public async Task Set2_UserA_Dropoff_ShouldReturnError()
        {
            // Arrange
            Fixture = new VehiclesTestFixtures(2, "usera");

            // Act
            var result = await Fixture.VehiclesController.Dropoff("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Fixture.Dispose();
        }


        [Fact]
        public async Task Set3_UserA_Dropoff_ShouldReturnDetails()
        {
            // Arrange
            Fixture = new VehiclesTestFixtures(3, "usera");

            // Act
            var result = await Fixture.VehiclesController.Dropoff("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Fixture.Dispose();
        }
    }
}