﻿using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DriveHubTests
{
    public class VehicleTests_SET6
    {
        VehiclesTestFixtures Fixture;

        [Fact]
        public async Task Set6_UserA_Pickup_ShouldRedirectToSearch()
        {
            // Arrange
            Fixture = new VehiclesTestFixtures(6, "usera");

            // Act
            var result = await Fixture.VehiclesController.Pickup("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Search", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set6_UserA_Dropoff_ShouldReturnError()
        {
            // Arrange
            Fixture = new VehiclesTestFixtures(6, "usera");

            // Act
            var result = await Fixture.VehiclesController.Dropoff("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set6_UserB_Pickup_ShouldReturnSearch()
        {
            // Arrange
            Fixture = new VehiclesTestFixtures(6, "userb");

            // Act
            var result = await Fixture.VehiclesController.Pickup("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Search", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Set6_UserB_Dropoff_ShouldReturnError()
        {
            // Arrange
            Fixture = new VehiclesTestFixtures(6, "userb");

            // Act
            var result = await Fixture.VehiclesController.Dropoff("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }
    }
}