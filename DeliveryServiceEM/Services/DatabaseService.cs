using Npgsql;
using System;
using System.Collections.Generic;
using DeliveryServiceEM.Models;

namespace DeliveryServiceEM.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Order> GetOrders()
        {
            var orders = new List<Order>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand("SELECT * FROM \"Order\"", conn);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        Id = reader.GetInt32(0),
                        Weight = reader.GetFloat(1),
                        District = reader.GetString(2),
                        DeliveryTime = reader.GetDateTime(3)
                    });
                }
            }
            return orders;
        }

        public void ImportOrdersFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл {filePath} не найден.");

            var orders = new List<Order>();
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var fields = line.Split(';');
                if (fields.Length != 4)
                    throw new FormatException("Неверный формат строки в файле заказов.");

                var order = new Order
                {
                    Id = int.Parse(fields[0]),
                    Weight = (float)decimal.Parse(fields[1]),
                    District = fields[2],
                    DeliveryTime = DateTime.Parse(fields[3])
                };

                orders.Add(order);
            }

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                foreach (var order in orders)
                {
                    var sql = "INSERT INTO \"Order\" (weight, district, \"deliveryTime\") VALUES ( @weight, @district, @deliveryTime)";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("weight", order.Weight);
                        cmd.Parameters.AddWithValue("district", order.District);
                        cmd.Parameters.AddWithValue("deliveryTime", order.DeliveryTime);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void ExportOrdersToFile(string filePath)
        {
            var orders = GetOrders();

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("ID;Weight;District;DeliveryTime");
                foreach (var order in orders)
                {
                    writer.WriteLine($"{order.Id};{order.Weight};{order.District};{order.DeliveryTime:yyyy-MM-dd HH:mm:ss}");
                }
            }
        }
        public void SaveOrdersToFile(string filePath, List<Order> orders)
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
