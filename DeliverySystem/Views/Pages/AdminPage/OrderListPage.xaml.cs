using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DeliverySystem.Views.Windows;
using ModelDeliverySystemData.Model;

namespace DeliverySystem.Views.Pages.AdminPage
{
    public partial class OrderListPage : Page
    {
        private dbContext _context;
        private Order selectedOrder;
        public List<OrderViewModel> Orders { get; set; }

        public OrderListPage()
        {
            InitializeComponent();
            _context = new dbContext();
            DataContext = this;
            LoadOrders();
        }

        private void LoadOrders()
        {
            Orders = _context.Orders
                .Select(o => new OrderViewModel
                {
                    OrderID = o.OrderID,
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    ClientFullName = o.Client != null ? o.Client.FirstName + " " + o.Client.LastName : "Клиент не указан",
                    CourierFullName = _context.Couriers
                        .Where(c => c.CourierID == _context.Deliveries
                            .Where(d => d.OrderID == o.OrderID)
                            .Select(d => d.CourierID)
                            .FirstOrDefault())
                        .Select(c => c.FirstName + " " + c.LastName)
                        .FirstOrDefault() ?? "Курьер не назначен"
                })
                .ToList();

            OrdersListView.ItemsSource = Orders;
        }


        private void OrdersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersListView.SelectedItem != null)
            {
                var selectedOrderDetails = (OrderViewModel)OrdersListView.SelectedItem;
                selectedOrder = _context.Orders.FirstOrDefault(o => o.OrderID == selectedOrderDetails.OrderID);
            }
        }

        private void ShowOrderDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersListView.SelectedItem is OrderViewModel selectedOrderDetails)
            {
                var selectedOrder = _context.Orders.FirstOrDefault(o => o.OrderID == selectedOrderDetails.OrderID);
                var orderDetailsWindow = new OrderDetailsWindow(selectedOrder);
                orderDetailsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите заказ.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Реализация фильтрации списка заказов по тексту в SearchBox
            var searchText = SearchBox.Text.ToLower();
            var filteredOrders = Orders.Where(o =>
                (o.ClientFullName?.ToLower().Contains(searchText) ?? false) ||
                (o.CourierFullName?.ToLower().Contains(searchText) ?? false) ||
                (o.Status?.ToLower().Contains(searchText) ?? false)).ToList();
            OrdersListView.ItemsSource = filteredOrders;
        }


        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // Реализация фильтрации списка заказов по датам
            var startDate = StartDatePicker.SelectedDate;
            var endDate = EndDatePicker.SelectedDate;

            var filteredOrders = Orders.Where(o =>
                (!startDate.HasValue || o.OrderDate >= startDate) &&
                (!endDate.HasValue || o.OrderDate <= endDate)).ToList();

            OrdersListView.ItemsSource = filteredOrders;
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            // Сброс фильтров
            SearchBox.Text = string.Empty;
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            OrdersListView.ItemsSource = Orders;
        }
    }
}
