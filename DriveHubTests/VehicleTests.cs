﻿using DriveHubModel;
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
        VehiclesTestFixtures? Fixture = null;

        public override void Before(MethodInfo methodUnderTest) {  }

        public override void After(MethodInfo methodUnderTest)
        {
            Fixture = null;
        }

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
        }
    }
}