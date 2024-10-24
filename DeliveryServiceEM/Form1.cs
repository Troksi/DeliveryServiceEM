
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

        public Form1(Logger logger, DatabaseService connectionString)
        {
            InitializeComponent();

            _databaseService = connectionString;
            _logger = logger;

            LoadOrders();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            var orders = getFilteredOrders();

            dataGridViewOrders.DataSource = orders;
            _logger.Log($"������� {orders.Count} �������");
        }

        private List<Order> getFilteredOrders()
        {
            string district = txtDistrict.Text;
            DateTime selectedDate = dateTimePicker1.Value.Date;
            DateTime selectedTime = dateTimePicker3.Value;
            DateTime fullDateTimeFrom = selectedDate.Add(selectedTime.TimeOfDay);
            selectedDate = dateTimePicker2.Value.Date;
            selectedTime = dateTimePicker4.Value;
            DateTime fullDateTimeTo = selectedDate.Add(selectedTime.TimeOfDay);

            var orders = _databaseService.GetOrders();
            if (string.IsNullOrWhiteSpace(district))
            {
                dataGridViewOrders.DataSource = orders;
                _logger.Log($"������� {orders.Count} �������");
                return new List<Order>();
            }

            _logger.Log("���������� ������� ��� ������: " + district);
            var filteredOrders = orders
                .Where(order => order.District == district &&
                                order.DeliveryTime >= fullDateTimeFrom &&
                                order.DeliveryTime <= fullDateTimeTo)
                .OrderBy(order => order.DeliveryTime) // ��������� �� ������� �������� ��� ���������� ������
                .ToList();

            if (!filteredOrders.Any())
            {
                dataGridViewOrders.DataSource = new List<Order>();
                _logger.Log($"������� ��������� ��� ������: {district} � �������: C {fullDateTimeFrom} �� {fullDateTimeTo}");
                return new List<Order>();
            }

            DateTime firstOrderTime = filteredOrders.First().DeliveryTime;

            DateTime timeLimit = firstOrderTime.AddMinutes(30);
            var finalOrders = filteredOrders
                .Where(order => order.DeliveryTime <= timeLimit)
                .ToList();

            return finalOrders;
        }

        private void btnImportOrders_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt";
                openFileDialog.Title = "�������� ���� ��� ������� �������";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _databaseService.ImportOrdersFromFile(openFileDialog.FileName);
                        _logger.Log($"������ ������� �������������.");
                        MessageBox.Show("������ ������� �������������.", "�����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log($"������ ������� �������: {ex.Message}");
                        MessageBox.Show($"������ ������� �������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnExportOrders_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text files (*.txt)|*.txt";
                saveFileDialog.Title = "��������� ������ � ����";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _databaseService.ExportOrdersToFile(saveFileDialog.FileName);
                        _logger.Log($"������ ������� ��������������.");
                        MessageBox.Show("������ ������� ��������������.", "�����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log($"������ �������� �������: {ex.Message}");
                        MessageBox.Show($"������ �������� �������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
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

        private void CombineDateTimeFrom()
        {
            // �������� ���� � �����
            DateTime selectedDate = dateTimePicker1.Value.Date; // ������ ����, ����� ����������
            DateTime selectedTime = dateTimePicker3.Value;  // ������ �����

            // ����������� ���� � �����
            DateTime fullDateTimeFrom = selectedDate.Add(selectedTime.TimeOfDay);

            // ��� �������: ����� � �������, ���������� �������� - ������ � ��������� �����
            Console.WriteLine($"������� ���� � ����� � : {fullDateTimeFrom}");
        }

        private void CombineDateTimeTo()
        {
            // �������� ���� � �����
            DateTime selectedDate = dateTimePicker2.Value.Date; // ������ ����, ����� ����������
            DateTime selectedTime = dateTimePicker4.Value;  // ������ �����

            // ����������� ���� � �����
            DateTime fullDateTimeTo = selectedDate.Add(selectedTime.TimeOfDay);

            // ��� �������: ����� � �������, ���������� �������� - ������ � ��������� �����
            Console.WriteLine($"������� ���� � ����� �� : {fullDateTimeTo}");
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            CombineDateTimeFrom();
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            CombineDateTimeFrom();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            CombineDateTimeTo();
        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {
            CombineDateTimeTo();
        }

        private void SaveFilteredOrdersToFile(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Text files (*.txt)|*.txt";
                    saveFileDialog.Title = "��������� ������ � ����";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            var orders = getFilteredOrders();

                            _databaseService.SaveOrdersToFile(saveFileDialog.FileName, orders);
                            _logger.Log($"������ ������� ���������.");
                            MessageBox.Show("������ ������� ���������.", "�����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            _logger.Log($"������ ���������� �������: {ex.Message}");
                            MessageBox.Show($"������ ���������� �������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"��������� ������ ��� ���������� ������� � ����: {ex.Message}");
                MessageBox.Show($"��������� ������ ��� ���������� ������� � ����: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}
