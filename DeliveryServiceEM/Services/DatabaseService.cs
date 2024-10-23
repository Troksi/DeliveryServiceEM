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
    }
}
