using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModelDeliverySystemData.Model;
using Telegram.Bot;

namespace DeliverySystem.Views.Pages.AdminPage
{
    public partial class AssignCourierPage : Page
    {
        private dbContext _context;
        private Order _order;
        private bool isEditing = false;

        public AssignCourierPage(int orderId)
        {
            InitializeComponent();
            _context = new dbContext();
            _order = _context.Orders.Include("Client").FirstOrDefault(o => o.OrderID == orderId); // Загружаем объект из базы данных
            LoadComboBoxData();
            LoadOrderData();
            LoadAvailableCouriers();
        }


        private void LoadComboBoxData()
        {
            ClientComboBox.ItemsSource = _context.Clients.ToList();
            ClientComboBox.DisplayMemberPath = "FullName";
            ClientComboBox.SelectedValuePath = "ClientID";
        }

        private void LoadOrderData()
        {
            if (_order != null)
            {
                OrderIdTextBox.Text = _order.OrderID.ToString();
                OrderDateTextBox.Text = _order.OrderDate.ToString("yyyy-MM-dd");
                DeliveryDatePicker.SelectedDate = _order.DeliveryDate;

                StatusComboBox.SelectedItem = StatusComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == _order.Status);
                TotalAmountTextBox.Text = _order.TotalAmount.ToString("F2");

                PaymentMethodComboBox.SelectedItem = PaymentMethodComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == _order.PaymentMethod);
                CommentTextBox.Text = _order.Comment;

                var client = _order.Client;
                if (client != null)
                {
                    ClientComboBox.SelectedValue = client.ClientID;
                }

                var deliveryAddress = _context.DeliveryAddresses.FirstOrDefault(a => a.OrderID == _order.OrderID);
                if (deliveryAddress != null)
                {
                    DeliveryAddressTextBox.Text = $"{deliveryAddress.Address}, {deliveryAddress.City}, {deliveryAddress.Country}";
                }
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

        private void SaveChanges()
        {
            try
            {
                _order.OrderDate = DateTime.Parse(OrderDateTextBox.Text);
                _order.DeliveryDate = DeliveryDatePicker.SelectedDate ?? _order.DeliveryDate; // Устанавливаем текущую дату доставки, если новая не выбрана
                _order.Status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? _order.Status; // Сохраняем текущий статус, если новый не выбран
                _order.TotalAmount = decimal.Parse(TotalAmountTextBox.Text);
                _order.PaymentMethod = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? _order.PaymentMethod;
                _order.Comment = CommentTextBox.Text;
                _order.ClientID = (int)ClientComboBox.SelectedValue;

                var deliveryAddress = _context.DeliveryAddresses.FirstOrDefault(a => a.OrderID == _order.OrderID);
                if (deliveryAddress != null)
                {
                    deliveryAddress.Address = DeliveryAddressTextBox.Text;
                }

                _context.Entry(_order).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();

                MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении изменений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditing)
            {
                // Сохранение изменений
                SaveChanges();
                EditButton.Content = "Редактировать";
                SetReadOnlyFields(true);
            }
            else
            {
                // Включение режима редактирования
                EditButton.Content = "Сохранить";
                SetReadOnlyFields(false);
            }

            isEditing = !isEditing;
        }

        private void SetReadOnlyFields(bool isReadOnly)
        {
            DeliveryDatePicker.IsEnabled = !isReadOnly;
            StatusComboBox.IsEnabled = !isReadOnly;
            TotalAmountTextBox.IsReadOnly = isReadOnly;
            PaymentMethodComboBox.IsEnabled = !isReadOnly;
            CommentTextBox.IsReadOnly = isReadOnly;
            ClientComboBox.IsEnabled = !isReadOnly;
            DeliveryAddressTextBox.IsReadOnly = isReadOnly;

            DeliveryDatePicker.Background = isReadOnly ? Brushes.WhiteSmoke : Brushes.White;
            StatusComboBox.Background = isReadOnly ? Brushes.WhiteSmoke : Brushes.White;
            TotalAmountTextBox.Background = isReadOnly ? Brushes.WhiteSmoke : Brushes.White;
            PaymentMethodComboBox.Background = isReadOnly ? Brushes.WhiteSmoke : Brushes.White;
            CommentTextBox.Background = isReadOnly ? Brushes.WhiteSmoke : Brushes.White;
            ClientComboBox.Background = isReadOnly ? Brushes.WhiteSmoke : Brushes.White;
            DeliveryAddressTextBox.Background = isReadOnly ? Brushes.WhiteSmoke : Brushes.White;
        }
    }
}
