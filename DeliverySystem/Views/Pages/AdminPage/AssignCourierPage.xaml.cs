using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ModelDeliverySystemData.Model;
using Telegram.Bot;

namespace DeliverySystem.Views.Pages.AdminPage
{
    public partial class AssignCourierPage : Page
    {
        private dbContext _context;
        private Order _order;

        public AssignCourierPage(Order order)
        {
            InitializeComponent();
            _context = new dbContext();
            _order = order;
            LoadOrderData();
            LoadAvailableCouriers();
        }

        private void LoadOrderData()
        {
            OrderIdTextBox.Text = _order.OrderID.ToString();
            OrderDateTextBox.Text = _order.OrderDate.ToString();
            DeliveryDateTextBox.Text = _order.DeliveryDate.ToString();
            StatusTextBox.Text = _order.Status;
            TotalAmountTextBox.Text = _order.TotalAmount.ToString();
            PaymentMethodTextBox.Text = _order.PaymentMethod;
            CommentTextBox.Text = _order.Comment;

            var client = _context.Clients.Find(_order.ClientID);
            if (client != null)
            {
                ClientTextBox.Text = $"{client.FirstName} {client.LastName}";
            }

            var deliveryAddress = _context.DeliveryAddresses.FirstOrDefault(a => a.OrderID == _order.OrderID);
            if (deliveryAddress != null)
            {
                DeliveryAddressTextBox.Text = $"{deliveryAddress.Address}, {deliveryAddress.City}, {deliveryAddress.Country}";
            }
        }

        private void LoadAvailableCouriers()
        {
            var availableCouriers = _context.Couriers.Where(c => c.IsAvailable != false).ToList();
            CourierComboBox.ItemsSource = availableCouriers;
        }
        private async Task NotifyCourierViaTelegram(int courierId, int orderId)
        {
            try
            {
                var courier = _context.Couriers.FirstOrDefault(c => c.CourierID == courierId);
                if (courier != null && courier.TelegramChatId.HasValue)
                {
                    var botClient = new TelegramBotClient("7349407796:AAG5lk8cLNUEX8jpsJAIh98Y_HUqUbLGOUo"); // Замените на ваш токен
                    var orderDetails = _context.Orders
                                     .Include("OrderedItems") // Используйте правильное имя свойства
                                     .FirstOrDefault(o => o.OrderID == orderId);


                    if (orderDetails != null)
                    {
                        string messageText = $"Новый заказ ID: {orderDetails.OrderID}\n" +
                                             $"Дата доставки: {DateTime.Now}\n" +
                                             $"Итого: {orderDetails.TotalAmount}\n" +
                                             "Товары:\n" +
                                             string.Join("\n", orderDetails.OrderedItems.Select(oi => $"- {oi.ProductName} x {oi.Quantity}"));

                        await botClient.SendTextMessageAsync(courier.TelegramChatId.Value, messageText);
                        MessageBox.Show("Курьеру отправлено уведомление о новом заказе.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке уведомления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AssignCourier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CourierComboBox.SelectedItem is Courier selectedCourier)
                {
                    var delivery = new Delivery
                    {
                        OrderID = _order.OrderID,
                        CourierID = selectedCourier.CourierID,
                        DeliveryDate = DateTime.Now,
                        StatusID = 1 // Предположим, что статус 1 - это 'Запланировано'
                    };

                    _context.Deliveries.Add(delivery);
                    selectedCourier.IsAvailable = false;
                    _context.SaveChanges();

                    MessageBox.Show("Курьер успешно назначен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Отправка уведомления курьеру через Telegram
                    Task.Run(() => NotifyCourierViaTelegram(selectedCourier.CourierID, _order.OrderID));

                    NavigationService.GoBack();
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите курьера.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
