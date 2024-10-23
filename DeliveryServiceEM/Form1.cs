
using DeliveryServiceEM.Models;
using DeliveryServiceEM.Services;
using Microsoft.Extensions.Configuration;
using System.IO;


namespace DeliveryServiceEM
{

    public partial class Form1 : Form
    {
        private readonly DatabaseService _databaseService;
        private readonly Logger _logger;
        private List<Order> _orders;

        public Form1()
        {
            InitializeComponent();

            string appSettingsPath = "appsettings.json";
            EnsureAppSettingsFile(appSettingsPath);

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(appSettingsPath)
                .Build();

            string connectionString = config.GetConnectionString("DefaultConnection");
            string logPath = config["Logging:LogPath"];

            EnsureLogFile(logPath);

            _databaseService = new DatabaseService(connectionString);
            _logger = new Logger(logPath);

            LoadOrders();
        }

        private void EnsureAppSettingsFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                var defaultConfig = @"
            {
                ""ConnectionStrings"": {
                    ""DefaultConnection"": ""Host=localhost;Port=5432;Username=your_username;Password=your_password;Database=delivery_db""
                },
                ""Logging"": {
                    ""LogPath"": ""Logs/log.txt""
                }
            }";

                File.WriteAllText(filePath, defaultConfig);
                MessageBox.Show($"���� {filePath} ��� ������ � ���������� ����������. ����������, ��������� ���������.");
            }
        }

        private void EnsureLogFile(string logFilePath)
        {
            string logDirectory = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath).Close();  // ������� ������ ���� � ��������� �����
            }
        }


        private void btnFilter_Click(object sender, EventArgs e)
        {
            string district = txtDistrict.Text;
            DateTime startTime = dateTimePicker1.Value;

            var orders = _databaseService.GetOrders();
            if (string.IsNullOrWhiteSpace(district))
            {
                dataGridViewOrders.DataSource = orders;
                _logger.Log($"������� {orders.Count} �������");
                return;
            }

            _logger.Log("���������� ������� ��� ������: " + district);
            var filteredOrders = orders
                .Where(o => o.District == district)
                //.Where(o => o.DeliveryTime >= startTime && o.DeliveryTime >= startTime.AddMinutes(30))
                .ToList();

            dataGridViewOrders.DataSource = filteredOrders;
            _logger.Log($"������� {filteredOrders.Count} �������");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // ���������� ���������� ������� ��� ������ ������ � �������
            dataGridViewOrders.SelectionChanged += dataGridViewOrders_SelectionChanged;
        }
        private void LoadOrders()
        {
            try
            {
                _orders = _databaseService.GetOrders();
                dataGridViewOrders.DataSource = _orders;

                _logger.Log("��� ������ ��������� ��� ������ ����������.");
            }
            catch (Exception ex)
            {
                _logger.Log($"������ ��� �������� �������: {ex.Message}");
                MessageBox.Show("�� ������� ��������� ������. ��������� ����������� � ���� ������.");
            }
        }

        // ����� ��� ��������� ������ ������ � �������
        private void dataGridViewOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewOrders.SelectedRows.Count > 0)
            {
                // �������� ��������� ������
                var selectedRow = dataGridViewOrders.SelectedRows[0];

                // ��������� ������ � ������ (���������� ������)
                string district = selectedRow.Cells["District"].Value.ToString();
                _logger.Log($"������ �����: {district}");

                // ��������� �������� ������ � ��������� ���� ��� ����������
                txtDistrict.Text = district;
            }
        }

    }

}
