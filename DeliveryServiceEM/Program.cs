using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DeliveryServiceEM.Models;
using DeliveryServiceEM.Services;
using DeliveryServiceEM.Interfaces;

namespace DeliveryServiceEM
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Validator validator = new Validator();
                Config config = new ConfigurService(validator, "appsettings.json").GetConfigur();
                Logger logger = new Logger(config.LogPath);
                DatabaseService databaseService = new DatabaseService(config.ConnectionString);

                if (config.RunWithoutWindow)
                {
                    logger.Log("Приложение запущено в режиме без окон.");
                    var orderFilterService = new OrderFilterService(databaseService);
                    var filteredOrders = orderFilterService.FilterOrders(
                        config.CityDistrict, 
                        config.FirstDeliveryDateTime, 
                        config.FirstDeliveryDateTime.AddMinutes(30));

                    FileSaveService.SaveToFile(config.DeliveryOrderPath, filteredOrders);

                    logger.Log($"Фильтрация завершена. Найдено {filteredOrders.Count} заказов. Результат записан в {config.DeliveryOrderPath}.");
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1(logger, databaseService));

                }
            }
            catch (Exception ex)
            {
                // Логируем ошибки при инициализации программы
                File.AppendAllText("Logs/error_log.txt", $"{DateTime.Now}: Ошибка: {ex.Message} {Environment.NewLine}{ex.StackTrace}{Environment.NewLine}");
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Validator validator = new Validator();
                Config config = new ConfigurService(validator, "appsettings.json").GetConfigur();
                Logger logger = new Logger(config.LogPath);
                logger.Log($"Конец программы.");
            }
        }

    }
}