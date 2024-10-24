using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DeliveryServiceEM.Models;
using DeliveryServiceEM.Services;


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

                string appSettingsPath = "appsettings.json";
                validator.EnsureAppSettingsFile(appSettingsPath);

                var config = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile(appSettingsPath)
                   .Build();

                bool runWithoutWindow = validator.ReadBoolConfig(config, "AppSettings:RunWithoutWindow", false);
                string cityDistrict = validator.ReadStringConfig(config, "AppSettings:CityDistrict", "�����������");
                DateTime firstDeliveryDateTime = validator.ReadDateTimeConfig(config, "AppSettings:FirstDeliveryDateTime", DateTime.Now);
                string logPath;
                string orderPath = validator.ReadStringConfig(config, "AppSettings:DeliveryOrderPath", "Results/delivery_order.txt");
                validator.EnsureTextFile(orderPath);
                string connectionString = config.GetConnectionString("DefaultConnection");
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new Exception("������ ����������� � ���� ������ ����������� ��� �����.");

                var databaseService = new DatabaseService(connectionString);

                Logger logger;
                if (runWithoutWindow)
                {
                    logPath = validator.ReadStringConfig(config, "AppSettings:DeliveryLogPath", "Logs/delivery_log.txt");
                    validator.EnsureTextFile(logPath);
                    logger = new Logger(logPath);

                    logger.Log("���������� �������� � ������ ��� ����.");

                    List<Order> orders = databaseService.GetOrders();

                    var filteredOrders = orders
                        .Where(o => o.District == cityDistrict)
                        .Where(o => o.DeliveryTime >= firstDeliveryDateTime && o.DeliveryTime <= firstDeliveryDateTime.AddMinutes(30))
                        .ToList();

                    SaveOrdersToFile(filteredOrders, orderPath);

                    logger.Log($"���������� ���������. ������� {filteredOrders.Count} �������. ��������� ������� � {orderPath}.");
                }
                else
                {
                    logPath = validator.ReadStringConfig(config, "Logging:LogPath", "Logs/delivery_log.txt");
                    validator.EnsureTextFile(logPath);
                    logger = new Logger(logPath);

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1(logger, databaseService));

                }
            }
            catch (Exception ex)
            {
                // �������� ������ ��� ������������� ���������
                File.AppendAllText("Logs/error_log.txt", $"{DateTime.Now}: ������: {ex.Message} {Environment.NewLine}{ex.StackTrace}{Environment.NewLine}");
                MessageBox.Show($"��������� ������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Validator validator = new Validator();
                string appSettingsPath = "appsettings.json";
                validator.EnsureAppSettingsFile(appSettingsPath);

                var config = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile(appSettingsPath)
                   .Build();
                bool runWithoutWindow = validator.ReadBoolConfig(config, "AppSettings:RunWithoutWindow", false);
                if (runWithoutWindow)
                {
                    var logPath = validator.ReadStringConfig(config, "AppSettings:DeliveryLogPath", "Logs/delivery_log.txt");
                    var logger = new Logger(logPath);
                    logger.Log($"����� ���������.");

                }
                else
                {
                    var logPath = validator.ReadStringConfig(config, "Logging:LogPath", "Logs/delivery_log.txt");
                    var logger = new Logger(logPath);
                    logger.Log($"����� ���������.");
                }
            }
        }

        private static void SaveOrdersToFile(List<Order> orders, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("ID;Weight;District;DeliveryTime");
                foreach (var order in orders)
                {
                    writer.WriteLine($"{order.Id};{order.Weight};{order.District};{order.DeliveryTime:yyyy-MM-dd HH:mm:ss}");
                }
            }
        }

    }
}