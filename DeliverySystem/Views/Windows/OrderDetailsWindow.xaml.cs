using ModelDeliverySystemData.Model;
using System.Linq;
using System.Windows;

namespace DeliverySystem.Views.Windows
{
    public partial class OrderDetailsWindow : Window
    {
        private Order _order;

        public OrderDetailsWindow(Order order)
        {
            InitializeComponent();
            _order = order;
            LoadOrderDetails();
        }

        private void LoadOrderDetails()
        {
            OrderIDTextBlock.Text = _order.OrderID.ToString();
            OrderDateTextBlock.Text = _order.OrderDate.ToString();
            DeliveryDateTextBlock.Text = _order.DeliveryDate?.ToString() ?? "N/A";
            StatusTextBlock.Text = _order.Status;
            TotalAmountTextBlock.Text = _order.TotalAmount.ToString("C");

            if (_order.Client != null)
            {
                ClientTextBlock.Text = $"{_order.Client.FirstName} {_order.Client.LastName}";
            }
            else
            {
                ClientTextBlock.Text = "Клиент не указан";
            }

            var courier = _order.Deliveries.FirstOrDefault()?.Courier;
            if (courier != null)
            {
                CourierTextBlock.Text = $"{courier.FirstName} {courier.LastName}";
            }
            else
            {
                CourierTextBlock.Text = "Курьер не назначен";
            }

            OrderedItemsListBox.ItemsSource = _order.OrderedItems.Select(oi => new
            {
                oi.ProductName,
                oi.Quantity,
                oi.Price
            });
        }
    }
}
