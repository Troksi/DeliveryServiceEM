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
                    logger.Log("���������� �������� � ������ ��� ����.");
                    var orderFilterService = new OrderFilterService(databaseService);
                    var filteredOrders = orderFilterService.FilterOrders(
                        config.CityDistrict, 
                        config.FirstDeliveryDateTime, 
                        config.FirstDeliveryDateTime.AddMinutes(30));

                    FileSaveService.SaveToFile(config.DeliveryOrderPath, filteredOrders);

                    logger.Log($"���������� ���������. ������� {filteredOrders.Count} �������. ��������� ������� � {config.DeliveryOrderPath}.");
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
                // �������� ������ ��� ������������� ���������
                File.AppendAllText("Logs/error_log.txt", $"{DateTime.Now}: ������: {ex.Message} {Environment.NewLine}{ex.StackTrace}{Environment.NewLine}");
                MessageBox.Show($"��������� ������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Validator validator = new Validator();
                Config config = new ConfigurService(validator, "appsettings.json").GetConfigur();
                Logger logger = new Logger(config.LogPath);
                logger.Log($"����� ���������.");
            }
        }

    }
}