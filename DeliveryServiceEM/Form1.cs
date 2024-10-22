
using DeliveryServiceEM.Services;
using Microsoft.Extensions.Configuration;
using System.IO;


namespace DeliveryServiceEM
{

    public partial class Form1 : Form
    {
        private readonly DatabaseService _databaseService;
        private readonly Logger _logger;

        public Form1()
        {
            InitializeComponent();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = config.GetConnectionString("DefaultConnection");
            string logPath = config["Logging:LogPath"];

            _databaseService = new DatabaseService(connectionString);
            _logger = new Logger(logPath);
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            string district = txtDistrict.Text;
            DateTime startTime = dateTimePicker1.Value;

            _logger.Log("Фильтрация заказов для района: " + district);

            var orders = _databaseService.GetOrders();
            var filteredOrders = orders
                .Where(o => o.District == district)
                .Where(o => o.DeliveryTime >= startTime && o.DeliveryTime <= startTime.AddMinutes(30))
                .ToList();

            dataGridViewOrders.DataSource = filteredOrders;
            _logger.Log($"Найдено {filteredOrders.Count} заказов");
        }

    }


}
