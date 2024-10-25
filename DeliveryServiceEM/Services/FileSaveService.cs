using DeliveryServiceEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryServiceEM.Services
{
    public class FileSaveService
    {
        public static void SaveToFile(string filePath, List<Order> orders)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            try
            {
                CheckPathAndCreatFile(filePath);

                using (var writer = new StreamWriter(filePath))
                {
                    foreach (var order in orders)
                    {
                        writer.WriteLine($"{order.Id};{order.Weight};{order.District};{order.DeliveryTime:yyyy-MM-dd HH:mm:ss}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static void CheckPathAndCreatFile(string filePath)
        {
            string textDirectory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(textDirectory) && !string.IsNullOrEmpty(textDirectory))
            {
                Directory.CreateDirectory(textDirectory);
            }

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }
    }
}
