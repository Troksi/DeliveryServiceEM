using DeliveryServiceEM.Models;
using DeliveryServiceEM.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryServiceEMTests
{
    [TestClass]
    public class FileSaveTests
    {
        [TestMethod]
        public void SaveFilteredOrdersToFile_ShouldSaveOrdersToFile()
        {

            var filteredOrders = new List<Order>
        {
            new Order { Id = 1, District = "District1", DeliveryTime = new DateTime(2024, 10, 24, 10, 0, 0), Weight = 10 },
            new Order { Id = 2, District = "District1", DeliveryTime = new DateTime(2024, 10, 24, 10, 30, 0), Weight = 5 }
        };
            string filePath = "testOrders.txt";

            FileSaveService.SaveToFile(filePath, filteredOrders);

            Assert.IsTrue(File.Exists(filePath));

            var fileContent = File.ReadAllLines(filePath);
            Assert.AreEqual(2, fileContent.Length);
            Assert.IsTrue(fileContent[0].Contains("1;10;District1;2024-10-24 10:00:00"));
            Assert.IsTrue(fileContent[1].Contains("2;5;District1;2024-10-24 10:30:00"));

            // Clean up
            File.Delete(filePath);
        }

    }

}
