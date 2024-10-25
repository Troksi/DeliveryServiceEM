using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryServiceEM.Interfaces;
using DeliveryServiceEM.Models;
using DeliveryServiceEM.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DeliveryServiceEMTests
{
    [TestClass]
    public class OrderFilterServiceTests
    {
        private List<Order> GetTestOrders()
        {
            return new List<Order>
        {
            new Order { Id = 1, District = "District1", DeliveryTime = new DateTime(2024, 10, 24, 10, 0, 0), Weight = 10 },
            new Order { Id = 2, District = "District1", DeliveryTime = new DateTime(2024, 10, 24, 10, 30, 0), Weight = 5 },
            new Order { Id = 3, District = "District2", DeliveryTime = new DateTime(2024, 10, 24, 11, 0, 0), Weight = 3 },
            new Order { Id = 4, District = "District1", DeliveryTime = new DateTime(2024, 10, 24, 10, 15, 0), Weight = 2 }
        };
        }

        [TestMethod]
        public void FilterOrders_ShouldReturnCorrectOrders_WhenFilteringByDistrictAndTimeRange()
        {
            // Arrange
            var orders = GetTestOrders();
            var databaseServiceMock = new Mock<IDatabaseService>();
            databaseServiceMock.Setup(ds => ds.GetOrders()).Returns(orders);

            var orderFilterService = new OrderFilterService(databaseServiceMock.Object);
            DateTime startDateTime = new DateTime(2024, 10, 24, 10, 0, 0);
            DateTime endDateTime = new DateTime(2024, 10, 24, 12, 0, 0);
            string district = "District1";

            // Act
            var result = orderFilterService.FilterOrders(district, startDateTime, endDateTime);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.All(o => o.District == district));
            Assert.IsTrue(result.All(o => o.DeliveryTime >= startDateTime && o.DeliveryTime <= endDateTime));
        }

        [TestMethod]
        public void FilterOrders_ShouldReturnOrdersWithin30MinutesFromFirstOrder()
        {
            // Arrange
            var orders = GetTestOrders();
            var databaseServiceMock = new Mock<IDatabaseService>();
            databaseServiceMock.Setup(ds => ds.GetOrders()).Returns(orders);

            var orderFilterService = new OrderFilterService(databaseServiceMock.Object);
            DateTime startDateTime = new DateTime(2024, 10, 24, 10, 0, 0);
            DateTime endDateTime = new DateTime(2024, 10, 24, 12, 0, 0);
            string district = "District1";

            // Act
            var result = orderFilterService.FilterOrders(district, startDateTime, endDateTime);

            // Assert
            DateTime firstOrderTime = result.First().DeliveryTime;
            DateTime expectedEndTime = firstOrderTime.AddMinutes(30);
            Assert.IsTrue(result.All(o => o.DeliveryTime <= expectedEndTime));
        }

        [TestMethod]
        public void FilterOrders_ShouldReturnEmptyList_WhenNoOrdersInDistrict()
        {
            // Arrange
            var orders = GetTestOrders();
            var databaseServiceMock = new Mock<IDatabaseService>();
            databaseServiceMock.Setup(ds => ds.GetOrders()).Returns(orders);

            var orderFilterService = new OrderFilterService(databaseServiceMock.Object);
            DateTime startDateTime = new DateTime(2024, 10, 24, 10, 0, 0);
            DateTime endDateTime = new DateTime(2024, 10, 24, 12, 0, 0);
            string district = "NonExistentDistrict";

            // Act
            var result = orderFilterService.FilterOrders(district, startDateTime, endDateTime);

            // Assert
            Assert.AreEqual(0, result.Count);
        }
    }

}
