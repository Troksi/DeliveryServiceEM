using DeliveryServiceEM.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryServiceEM.Services
{
    public class ConfigurService
    {
        private readonly string _appSettingsPath;
        private readonly Validator _validator;
        public ConfigurService(Validator validator, string appSettingsPath)
        {
            _validator = validator;
            _appSettingsPath = appSettingsPath;
            ensureAppSettingsFile(appSettingsPath);
        }
        public Config GetConfigur()
        {
            Config config = new Config();
            var configJson = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(_appSettingsPath)
                .Build();

            config.RunWithoutWindow = _validator.ReadBoolConfig(configJson, "AppSettings:RunWithoutWindow", false);
            config.DeliveryOrderPath = _validator.ReadStringConfig(configJson, "AppSettings:DeliveryOrderPath", "Results/delivery_order.txt");
            config.LogPath = _validator.ReadStringConfig(
                configJson,
                config.RunWithoutWindow ? "AppSettings:DeliveryLogPath" : "Logging:LogPath",
                config.RunWithoutWindow ? "Logs/delivery_log.txt" : "Logs/log.txt");
            config.CityDistrict = _validator.ReadStringConfig(configJson, "AppSettings:CityDistrict", "Центральный");
            config.FirstDeliveryDateTime = _validator.ReadDateTimeConfig(configJson, "AppSettings:FirstDeliveryDateTime", DateTime.Now);
            
            FileSaveService.CheckPathAndCreatFile(config.DeliveryOrderPath);
            FileSaveService.CheckPathAndCreatFile(config.LogPath);

            config.ConnectionString = configJson.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(config.ConnectionString))
                throw new Exception("Строка подключения к базе данных отсутствует или пуста.");

            return config;
        }

        private void ensureAppSettingsFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                var defaultConfig = @"{
                                      ""ConnectionStrings"": {
                                        ""DefaultConnection"": ""Host=localhost;Port=5432;Username=your_username;Password=your_password;Database=DeliveryServiceEM""
                                                              },
                                      ""Logging"": {
                                        ""LogPath"": ""Logs/log.txt""
                                                    },
                                      ""AppSettings"": {
                                        ""RunWithoutWindow"": false,
                                        ""CityDistrict"": ""Центральный"",
                                        ""FirstDeliveryDateTime"": ""2024-10-22 09:00:00"",
                                        ""DeliveryLogPath"": ""Logs/delivery_log.txt"",
                                        ""DeliveryOrderPath"": ""Results/delivery_order.txt""
                                                        }
                                        }";

                File.WriteAllText(filePath, defaultConfig);
                MessageBox.Show($"Файл {filePath} был создан с дефолтными значениями. Пожалуйста, проверьте настройки.");
            }
        }

    }
}
