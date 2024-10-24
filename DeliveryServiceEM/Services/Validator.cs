using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DeliveryServiceEM.Services
{
    public class Validator
    {
        public string ReadStringConfig(IConfiguration config, string key, string defaultValue)
        {
            try
            {
                string value = config[key];
                if (string.IsNullOrWhiteSpace(value))
                    throw new Exception($"Значение для ключа '{key}' отсутствует или пусто.");
                return value;
            }
            catch (Exception ex)
            {
                File.AppendAllText("Logs/error_log.txt", $"Ошибка при чтении конфигурации '{key}': {ex.Message}{Environment.NewLine}");
                return defaultValue;
            }
        }

        // Метод для чтения параметра типа bool с обработкой ошибок
        public bool ReadBoolConfig(IConfiguration config, string key, bool defaultValue)
        {
            try
            {
                if (bool.TryParse(config[key], out bool result))
                {
                    return result;
                }
                else
                {
                    throw new Exception($"Неверный формат значения для ключа '{key}'. Ожидалось значение true/false.");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("Logs/error_log.txt", $"Ошибка при чтении конфигурации '{key}': {ex.Message}{Environment.NewLine}");
                return defaultValue;
            }
        }

        // Метод для чтения параметра типа DateTime с обработкой ошибок
        public DateTime ReadDateTimeConfig(IConfiguration config, string key, DateTime defaultValue)
        {
            try
            {
                if (DateTime.TryParse(config[key], out DateTime result))
                {
                    return result;
                }
                else
                {
                    throw new Exception($"Неверный формат даты для ключа '{key}'. Ожидался формат yyyy-MM-dd HH:mm:ss.");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("Logs/error_log.txt", $"Ошибка при чтении конфигурации '{key}': {ex.Message}{Environment.NewLine}");
                return defaultValue;
            }
        }

        public void EnsureAppSettingsFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                var defaultConfig = @"{
                                      ""ConnectionStrings"": {
                                        ""DefaultConnection"": ""Host=localhost;Port=5432;Username=your_username;Password=your_password;Database=delivery_db""
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

        public void EnsureTextFile(string textFilePath)
        {
            string textDirectory = Path.GetDirectoryName(textFilePath);
            if (!Directory.Exists(textDirectory))
            {
                Directory.CreateDirectory(textDirectory);
            }

            if (!File.Exists(textFilePath))
            {
                File.Create(textFilePath).Close();  // Создаем пустой файл и закрываем поток
            }
        }
    }
}
