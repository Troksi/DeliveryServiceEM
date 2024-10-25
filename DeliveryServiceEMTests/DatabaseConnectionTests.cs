using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;

namespace DeliveryServiceEMTests
{
    [TestClass]
    public class DatabaseConnectionTests
    {
        private string _connectionString = "Host=localhost;Username=postgres;Password=your_password;Database=DeliveryServiceEM";

        [TestMethod]
        public void CanConnectToDatabase_ShouldReturnTrue_WhenConnectionIsSuccessful()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                Assert.AreEqual(ConnectionState.Open, connection.State);
            }
        }

        [TestMethod]
        public void OrderTableSchema_ShouldMatchExpectedSchema()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand("SELECT column_name, data_type, is_nullable FROM information_schema.columns WHERE table_name = 'Order';", connection);
                using (var reader = command.ExecuteReader())
                {
                    // Словарь для хранения ожидаемых значений по имени колонки
                    var expectedColumns = new Dictionary<string, (string DataType, bool IsNullable)>
                {
                    { "order_id", ("integer", false) },
                    { "weight", ("real", false) },
                    { "district", ("character varying", false) },
                    { "deliveryTime", ("timestamp with time zone", false) }
                };

                    while (reader.Read())
                    {
                        string columnName = reader.GetString(0);
                        string dataType = reader.GetString(1);
                        bool isNullable = reader.GetString(2) == "YES";

                        Assert.IsTrue(expectedColumns.ContainsKey(columnName), $"Column {columnName} does not exist in the expected schema.");

                        var expectedColumn = expectedColumns[columnName];
                        Assert.AreEqual(expectedColumn.DataType, dataType, $"Data type for column {columnName} does not match expected type.");
                        Assert.AreEqual(expectedColumn.IsNullable, isNullable, $"Nullable property for column {columnName} does not match expected value.");
                    }
                }
            }
        }

        [TestMethod]
        public void OrderModel_ShouldMatchDatabaseSchema()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand("SELECT column_name, data_type FROM information_schema.columns WHERE table_name = 'Order';", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string columnName = reader.GetString(0);
                        string dataType = reader.GetString(1);

                        switch (columnName)
                        {
                            case "order_id":
                                Assert.AreEqual("integer", dataType, "Type mismatch for column 'order_id'");
                                break;
                            case "weight":
                                Assert.AreEqual("real", dataType, "Type mismatch for column 'weight'");
                                break;
                            case "district":
                                Assert.AreEqual("character varying", dataType, "Type mismatch for column 'district'");
                                break;
                            case "deliveryTime":
                                Assert.AreEqual("timestamp with time zone", dataType, "Type mismatch for column 'deliveryTime'");
                                break;
                            default:
                                Assert.Fail($"Unexpected column {columnName} found in database schema.");
                                break;
                        }
                    }
                }
            }
        }
    }

}
