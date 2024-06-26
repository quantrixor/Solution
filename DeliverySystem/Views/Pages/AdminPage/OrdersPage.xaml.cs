using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Toolkit.Uwp.Notifications; // Необходимо для отправки уведомлений
using ModelDeliverySystemData.Model;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Extensions.Logging;

namespace DeliverySystem.Views.Pages.AdminPage
{
    public partial class OrdersPage : Page
    {
        private HubConnection _hubConnection;
        private dbContext _context;
        private ObservableCollection<OrderViewModel> _orders;

        public OrdersPage()
        {
            InitializeComponent();
            _context = new dbContext();
            InitializeDatabaseContext();
            _orders = new ObservableCollection<OrderViewModel>();
            LoadOrders();
            InitializeSignalR();
        }

        private async void InitializeSignalR()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5212/orderHub")
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .Build();

            _hubConnection.On<string>("ReceiveOrderNotification", async (message) =>
            {
                ShowToastNotification("Новое уведомление", message);
                await Dispatcher.InvokeAsync(() => LoadOrders()); // Обновляем заказы в UI потоке
            });

            try
            {
                await _hubConnection.StartAsync();
                Console.WriteLine("SignalR подключение установлено.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к SignalR: {ex.Message}");
                Console.WriteLine($"Ошибка подключения к SignalR: {ex.Message}");
            }

            _hubConnection.Closed += async (error) =>
            {
                MessageBox.Show($"Соединение закрыто: {error?.Message}");
                Console.WriteLine($"Соединение закрыто: {error?.Message}");
                await Task.Delay(5000);
                await _hubConnection.StartAsync();
            };
        }


        private void ShowToastNotification(string title, string message)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }

        private void InitializeDatabaseContext()
        {
            try
            {
                _context = new dbContext();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize database context: {ex.Message}");
            }
        }

        private void LoadOrders()
        {

            var orders = _context.Orders.ToList();
            _orders.Clear();
            foreach (var order in orders)
            {
                _orders.Add(new OrderViewModel
                {
                    OrderID = order.OrderID,
                    UserID = order.User?.UserID ?? 0,
                    OrderDate = order.OrderDate,
                    DeliveryDate = order.DeliveryDate,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    PaymentMethod = order.PaymentMethod,
                    PaymentDate = order.PaymentDate,
                    Comment = order.Comment
                });
            }

            OrdersListView.ItemsSource = _orders;
        }

        private void ApplyFilters()
        {
            if (_context == null)
            {
                MessageBox.Show("Database context is not initialized.");
                return;
            }

            var filteredOrders = _context.Orders.AsQueryable();

            // Фильтрация по текстовому полю
            if (!string.IsNullOrEmpty(SearchBox.Text))
            {
                filteredOrders = filteredOrders.Where(o => o.Comment.Contains(SearchBox.Text));
            }

            // Фильтрация по дате заказа
            if (OrderDatePicker.SelectedDate.HasValue)
            {
                var selectedDate = OrderDatePicker.SelectedDate.Value.Date;
                filteredOrders = filteredOrders.Where(o => DbFunctions.TruncateTime(o.OrderDate) == selectedDate);
            }

            // Фильтрация по статусу
            var selectedStatus = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (selectedStatus != "Все")
            {
                filteredOrders = filteredOrders.Where(o => o.Status == selectedStatus);
            }

            _orders.Clear();
            foreach (var order in filteredOrders.ToList())
            {
                _orders.Add(new OrderViewModel
                {
                    OrderID = order.OrderID,
                    UserID = order.User?.UserID ?? 0,
                    OrderDate = order.OrderDate,
                    DeliveryDate = order.DeliveryDate,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    PaymentMethod = order.PaymentMethod,
                    PaymentDate = order.PaymentDate,
                    Comment = order.Comment
                });
            }

            OrdersListView.ItemsSource = _orders;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void OrdersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Логика обработки выбора заказа
        }

        private void AssignCourier_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = (OrderViewModel)OrdersListView.SelectedItem;
            if (selectedOrder != null)
            {
                // Найти полный объект заказа по его ID
                var order = _context.Orders.Include(o => o.Client).Include(o => o.OrderedItems).FirstOrDefault(o => o.OrderID == selectedOrder.OrderID);
                if (order != null)
                {
                    // Переход на страницу назначения курьера
                    NavigationService.Navigate(new AssignCourierPage(order));
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите заказ.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelOrder_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = (OrderViewModel)OrdersListView.SelectedItem;
            if (selectedOrder != null)
            {
                // Логика отмены выбранного заказа
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            OrderDatePicker.SelectedDate = null;
            StatusComboBox.SelectedIndex = 0;
            SearchBox.Text = string.Empty;
        }


    }

    public class OrderViewModel
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Comment { get; set; }
    }
}
