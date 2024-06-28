using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using ModelDeliverySystemData.Model;

namespace DeliverySystem.Views.Pages.AdminPage
{
    public partial class SalesStatisticsPage : Page
    {
        private dbContext _context;

        public SalesStatisticsPage()
        {
            InitializeComponent();
            _context = new dbContext();
        }

        public SeriesCollection SalesValues { get; set; }
        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        private void ShowChartButton_Click(object sender, RoutedEventArgs e)
        {
            if (QuarterComboBox.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Tag.ToString(), out int quarter))
            {
                DateTime startDate, endDate;
                switch (quarter)
                {
                    case 1:
                        startDate = new DateTime(DateTime.Now.Year, 1, 1);
                        endDate = new DateTime(DateTime.Now.Year, 3, 31);
                        break;
                    case 2:
                        startDate = new DateTime(DateTime.Now.Year, 4, 1);
                        endDate = new DateTime(DateTime.Now.Year, 6, 30);
                        break;
                    case 3:
                        startDate = new DateTime(DateTime.Now.Year, 7, 1);
                        endDate = new DateTime(DateTime.Now.Year, 9, 30);
                        break;
                    case 4:
                        startDate = new DateTime(DateTime.Now.Year, 10, 1);
                        endDate = new DateTime(DateTime.Now.Year, 12, 31);
                        break;
                    default:
                        return;
                }

                var salesData = GetSalesData(startDate, endDate);
                DisplayChart(salesData);
                DisplayStatistics(salesData);
            }
        }

        private Dictionary<int, decimal> GetSalesData(DateTime startDate, DateTime endDate)
        {
            var salesData = _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => o.OrderDate.Month)
                .ToDictionary(g => g.Key, g => g.Sum(o => o.TotalAmount));

            // Добавим вывод данных для отладки
            foreach (var data in salesData)
            {
                Console.WriteLine($"Месяц: {data.Key}, Продажи: {data.Value}");
            }

            return salesData;
        }

        private void DisplayChart(Dictionary<int, decimal> salesData)
        {
            if (salesData == null || salesData.Count == 0)
            {
                MessageBox.Show("Нет данных для выбранного квартала.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Console.WriteLine("Отображаемые данные для графика:");
            foreach (var data in salesData)
            {
                Console.WriteLine($"Месяц: {data.Key}, Продажи: {data.Value}");
            }

            SalesValues = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Продажи",
                    Values = new ChartValues<decimal>(salesData.Values),
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 15
                }
            };

            Labels = salesData.Keys.Select(m => new DateTime(DateTime.Now.Year, m, 1).ToString("MMMM")).ToList();
            Formatter = value => value.ToString("C");

            SalesChart.Series = SalesValues;

            SalesChart.AxisX.Clear();
            SalesChart.AxisX.Add(new Axis
            {
                Title = "Месяц",
                Labels = Labels,
                Separator = new LiveCharts.Wpf.Separator { Step = 1, IsEnabled = false }
            });

            SalesChart.AxisY.Clear();
            SalesChart.AxisY.Add(new Axis
            {
                Title = "Сумма продаж",
                LabelFormatter = Formatter,
                Separator = new LiveCharts.Wpf.Separator()
            });

            DataContext = this;
        }

        private void DisplayStatistics(Dictionary<int, decimal> salesData)
        {
            var totalSales = salesData.Values.Sum();
            var averageOrderValue = salesData.Values.Average();
            var totalOrders = salesData.Count;

            TotalSalesTextBlock.Text = totalSales.ToString("C");
            AverageOrderValueTextBlock.Text = averageOrderValue.ToString("C");
            TotalOrdersTextBlock.Text = totalOrders.ToString();
        }
    }
}
