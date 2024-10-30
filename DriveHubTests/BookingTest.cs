using DriveHubModel;
using DriveHub.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DriveHubTests
{
    public class BookingTests
    {
        [Fact]
        public async Task Set1_UserA_Search_ShouldRedirectToCurrent()
        {
            var bookingTestFixtures = new BookingTestFixtures(1, "usera");

            // Act
            var result = await bookingTestFixtures.Controller.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Current", viewResult.ViewName);
        }

        [Fact]
        public async Task Set1_UserA_Cancel_ShouldReturnCancelViewForIronStallion()
        {
            // Arrange
            var bookingTestFixtures = new BookingTestFixtures(1, "usera");

            // Act
            var result = await bookingTestFixtures.Controller.Cancel("b8075e83-6e70-4dee-b76a-22e8c7ee7ec1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Booking>(viewResult.Model);
            Assert.Equal("cac6a77c-59fd-4d0e-b557-9a3230a79e9a", model.VehicleId);
        }


        [Fact]
        public async Task Set1_UserA_Create_ShouldRedirectToCurrent()
        {
            var bookingTestFixtures = new BookingTestFixtures(1, "usera");

            // Act
            var result = await bookingTestFixtures.Controller.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Current", viewResult.ViewName);
        }

        [Fact]
        public async Task Set1_UserB_Search_ShouldReturn9Vehicles()
        {
            var bookingTestFixtures = new BookingTestFixtures(1, "userb");

            // Act
            var result = await bookingTestFixtures.Controller.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.Equal(9, model.Pods.Count);
        }

        [Fact]
        public async Task Set1_UserB_Search_CannotFindIronStallion()
        {
            var bookingTestFixtures = new BookingTestFixtures(1, "userb");

            // Act
            var result = await bookingTestFixtures.Controller.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.False(model.Pods.Where(c => c.VehicleId == "cac6a77c-59fd-4d0e-b557-9a3230a79e9a").Any());
        }

        [Fact]
        public async Task Set1_UserB_Create_ShoudReturnCreateView()
        {
            var bookingTestFixtures = new BookingTestFixtures(1, "userb");

            // Act
            var result = await bookingTestFixtures.Controller.Create("eeb7b72c-b362-4513-b84b-baa954c83ce0");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", viewResult.ViewName);
        }

        [Fact]
        public async Task Set1_UserB_Create_IronStallionRedirectsToError()
        {
            var bookingTestFixtures = new BookingTestFixtures(1, "userb");

            // Act
            var result = await bookingTestFixtures.Controller.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task Set2_UserA_Create_ShouldRedirectToCurrent()
        {
            var bookingTestFixtures = new BookingTestFixtures(2, "usera");

            // Act
            var result = await bookingTestFixtures.Controller.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Current", viewResult.ViewName);
        }

        [Fact]
        public async Task Set2_UserB_Search_ShouldReturn9Vehicles()
        {
            var bookingTestFixtures = new BookingTestFixtures(2, "userb");

            // Act
            var result = await bookingTestFixtures.Controller.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.Equal(9, model.Pods.Count);
        }

        [Fact]
        public async Task Set2_UserA_Search_ShouldRedirectToCurrent()
        {
            var bookingTestFixtures = new BookingTestFixtures(2, "usera");

            // Act
            var result = await bookingTestFixtures.Controller.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Current", viewResult.ViewName);
        }

        [Fact]
        public async Task Set2_UserB_Search_CannotFindIronStallion()
        {
            var bookingTestFixtures = new BookingTestFixtures(2, "userb");

            // Act
            var result = await bookingTestFixtures.Controller.Search();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BookingSearchVM>(viewResult.Model);
            Assert.False(model.Pods.Where(c => c.VehicleId == "cac6a77c-59fd-4d0e-b557-9a3230a79e9a").Any());
        }

        [Fact]
        public async Task Set2_UserB_Create_IronStallionRedirectsToError()
        {
            var bookingTestFixtures = new BookingTestFixtures(2, "userb");

            // Act
            var result = await bookingTestFixtures.Controller.Create("cac6a77c-59fd-4d0e-b557-9a3230a79e9a");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }
    }
}
