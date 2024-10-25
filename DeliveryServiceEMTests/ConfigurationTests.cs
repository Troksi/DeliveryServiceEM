using DeliveryServiceEM.Models;
using DeliveryServiceEM.Services;
using Microsoft.Extensions.Configuration;
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
    public class ConfigurationTests
    {
        [TestMethod]
        public void Configuration_ShouldLoadCorrectDeliveryPaths()
        {
            string filePath = "appsettings_test.json";
            Validator validator = new Validator();
            Config config = new ConfigurService(validator, filePath).GetConfigur();

            // Assert
            Assert.AreEqual("Host=localhost;Port=5432;Username=your_username;Password=your_password;Database=delivery_db", config.ConnectionString);
            Assert.AreEqual(false, config.RunWithoutWindow);
            Assert.AreEqual("Центральный", config.CityDistrict);
            Assert.AreEqual(DateTime.Parse("2024 - 10 - 22 09:00:00"), config.FirstDeliveryDateTime);
            Assert.AreEqual("Logs/log.txt", config.LogPath);
            Assert.AreEqual("Results/delivery_order.txt", config.DeliveryOrderPath);

            File.Delete(filePath);
        }

        //[TestMethod]
        //[ExpectedException(typeof(FormatException))]
        //public void Configuration_ShouldThrowException_WhenDateFormatIsInvalid()
        //{
        //    // Arrange
        //    var config = new ConfigurationBuilder()
        //        .AddJsonFile("invalidAppsettings_test.json") // Тестовый файл с некорректной датой
        //        .Build();

        //    // Act
        //    DateTime firstDeliveryDate = DateTime.Parse(config["AppSettings:FirstDeliveryDateTime"]);
        //}
    }

}
