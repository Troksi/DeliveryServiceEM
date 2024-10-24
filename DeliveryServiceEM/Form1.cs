
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
            _logger.Log($"Найдено {orders.Count} заказов");
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
                _logger.Log($"Найдено {orders.Count} заказов");
                return new List<Order>();
            }

            _logger.Log("Фильтрация заказов для района: " + district);
            var filteredOrders = orders
                .Where(order => order.District == district &&
                                order.DeliveryTime >= fullDateTimeFrom &&
                                order.DeliveryTime <= fullDateTimeTo)
                .OrderBy(order => order.DeliveryTime) // Сортируем по времени доставки для дальнейшей работы
                .ToList();

            if (!filteredOrders.Any())
            {
                dataGridViewOrders.DataSource = new List<Order>();
                _logger.Log($"Заказов ненайдено для пункта: {district} и времени: C {fullDateTimeFrom} По {fullDateTimeTo}");
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
                openFileDialog.Title = "Выберите файл для импорта заказов";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _databaseService.ImportOrdersFromFile(openFileDialog.FileName);
                        _logger.Log($"Заказы успешно импортированы.");
                        MessageBox.Show("Заказы успешно импортированы.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log($"Ошибка импорта заказов: {ex.Message}");
                        MessageBox.Show($"Ошибка импорта заказов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnExportOrders_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text files (*.txt)|*.txt";
                saveFileDialog.Title = "Сохранить заказы в файл";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _databaseService.ExportOrdersToFile(saveFileDialog.FileName);
                        _logger.Log($"Заказы успешно экспортированы.");
                        MessageBox.Show("Заказы успешно экспортированы.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log($"Ошибка экспорта заказов: {ex.Message}");
                        MessageBox.Show($"Ошибка экспорта заказов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Подключаем обработчик события для выбора строки в таблице
            dataGridViewOrders.SelectionChanged += dataGridViewOrders_SelectionChanged;
        }
        private void LoadOrders()
        {
            try
            {
                _orders = _databaseService.GetOrders();
                dataGridViewOrders.DataSource = _orders;

                _logger.Log("Все заказы загружены при старте приложения.");
            }
            catch (Exception ex)
            {
                _logger.Log($"Ошибка при загрузке заказов: {ex.Message}");
                MessageBox.Show("Не удалось загрузить заказы. Проверьте подключение к базе данных.");
            }
        }

        // Метод для обработки выбора строки в таблице
        private void dataGridViewOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewOrders.SelectedRows.Count > 0)
            {
                // Получаем выбранную строку
                var selectedRow = dataGridViewOrders.SelectedRows[0];

                // Извлекаем данные о районе (населенном пункте)
                string district = selectedRow.Cells["District"].Value.ToString();
                _logger.Log($"Выбран пункт: {district}");

                // Вставляем название района в текстовое поле для фильтрации
                txtDistrict.Text = district;
            }
        }

        private void CombineDateTimeFrom()
        {
            // Получаем дату и время
            DateTime selectedDate = dateTimePicker1.Value.Date; // Только дата, время обнуляется
            DateTime selectedTime = dateTimePicker3.Value;  // Только время

            // Комбинируем дату и время
            DateTime fullDateTimeFrom = selectedDate.Add(selectedTime.TimeOfDay);

            // Для примера: вывод в консоль, дальнейшие действия - работа с выбранной датой
            Console.WriteLine($"Выбрана дата и время С : {fullDateTimeFrom}");
        }

        private void CombineDateTimeTo()
        {
            // Получаем дату и время
            DateTime selectedDate = dateTimePicker2.Value.Date; // Только дата, время обнуляется
            DateTime selectedTime = dateTimePicker4.Value;  // Только время

            // Комбинируем дату и время
            DateTime fullDateTimeTo = selectedDate.Add(selectedTime.TimeOfDay);

            // Для примера: вывод в консоль, дальнейшие действия - работа с выбранной датой
            Console.WriteLine($"Выбрана дата и время По : {fullDateTimeTo}");
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
                    saveFileDialog.Title = "Сохранить заказы в файл";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            var orders = getFilteredOrders();

                            _databaseService.SaveOrdersToFile(saveFileDialog.FileName, orders);
                            _logger.Log($"Заказы успешно сохранены.");
                            MessageBox.Show("Заказы успешно сохранены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            _logger.Log($"Ошибка сохранения заказов: {ex.Message}");
                            MessageBox.Show($"Ошибка сохранения заказов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Произошла ошибка при сохранении заказов в файл: {ex.Message}");
                MessageBox.Show($"Произошла ошибка при сохранении заказов в файл: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}
