using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModelDeliverySystemData.Model;

namespace DeliverySystem.Views.Pages.AdminPage
{
    public partial class CreateOrderPage : Page, INotifyPropertyChanged
    {
        private dbContext _context;
        private decimal _totalAmount;
        private List<ProductOrder> _productOrders;

        public event PropertyChangedEventHandler PropertyChanged;

        public CreateOrderPage()
        {
            InitializeComponent();
            _context = new dbContext();
            _productOrders = new List<ProductOrder>();
            LoadComboBoxData();
            LoadProductList();
            DataContext = this;
        }

        public decimal TotalAmount
        {
            get { return _totalAmount; }
            set
            {
                _totalAmount = value;
                OnPropertyChanged("TotalAmount");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadComboBoxData()
        {
            ClientComboBox.ItemsSource = _context.Clients.ToList();
            ClientComboBox.DisplayMemberPath = "FullName";
            ClientComboBox.SelectedValuePath = "ClientID";
        }

        private void LoadProductList()
        {
            _productOrders = _context.Products
                .Select(p => new ProductOrder { ProductID = p.ProductID, ProductName = p.ProductName, Price = p.Price, Quantity = 0 })
                .ToList();
            ProductListBox.ItemsSource = _productOrders;
        }

        private void ClientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientComboBox.SelectedValue is int clientId)
            {
                var client = _context.Clients.Include("Cities").Include("Regions").FirstOrDefault(c => c.ClientID == clientId);
                if (client != null)
                {
                    RegionTextBox.Text = client.Regions?.RegionName;
                    CityTextBox.Text = client.Cities?.CityName;
                    DeliveryAddressTextBox.Text = client?.StreetAddress;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создание нового заказа
                var newOrder = new Order
                {
                    OrderDate = OrderDatePicker.SelectedDate ?? DateTime.Now,
                    DeliveryDate = DeliveryDatePicker.SelectedDate,
                    Status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    PaymentMethod = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    Comment = CommentTextBox.Text,
                    ClientID = (int)ClientComboBox.SelectedValue,
                    TotalAmount = 0, // Будет обновлено позже
                    CreatedBy = App.CurrentUser.UserID,
                    ManagedBy = App.CurrentUser.UserID
                };

                _context.Orders.Add(newOrder);
                _context.SaveChanges();

                // Создание элементов заказа (OrderedItems)
                decimal totalAmount = 0;
                foreach (var productOrder in _productOrders)
                {
                    if (productOrder.Quantity > 0)
                    {
                        var orderedItem = new OrderedItem
                        {
                            OrderID = newOrder.OrderID,
                            ProductID = productOrder.ProductID,
                            ProductName = productOrder.ProductName,
                            Quantity = productOrder.Quantity,
                            Price = productOrder.Price
                        };
                        totalAmount += productOrder.Price * productOrder.Quantity;
                        _context.OrderedItems.Add(orderedItem);
                    }
                }

                // Обновление общей суммы заказа
                newOrder.TotalAmount = totalAmount;

                // Добавление адреса доставки
                var deliveryAddress = new DeliveryAddress
                {
                    OrderID = newOrder.OrderID,
                    UserID = App.CurrentUser.UserID,
                    Address = DeliveryAddressTextBox.Text,
                    City = CityTextBox.Text,
                    PostalCode = DeliveryZipCodeTextBox.Text,
                    Country = RegionTextBox.Text // Регион теперь указан в поле Country
                };
                _context.DeliveryAddresses.Add(deliveryAddress);

                _context.SaveChanges();

                MessageBox.Show("Заказ успешно создан.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void DeliveryZipCodeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void DeliveryZipCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Ограничиваем длину до 7 символов
            if (DeliveryZipCodeTextBox.Text.Length > 7)
            {
                DeliveryZipCodeTextBox.Text = DeliveryZipCodeTextBox.Text.Substring(0, 7);
                DeliveryZipCodeTextBox.CaretIndex = DeliveryZipCodeTextBox.Text.Length; // Устанавливаем курсор в конец текста
            }
        }

        private void QuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void QuantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is ProductOrder productOrder)
            {
                if (int.TryParse(textBox.Text, out int quantity))
                {
                    productOrder.Quantity = quantity;
                    UpdateTotalAmount();
                }
            }
        }

        private void UpdateTotalAmount()
        {
            TotalAmount = _productOrders.Sum(po => po.Price * po.Quantity);
        }

        private void ProductListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var selectedItem in e.AddedItems)
            {
                if (selectedItem is ProductOrder productOrder)
                {
                    if (productOrder.Quantity == 0)
                    {

                        productOrder.Quantity = 1;
                        UpdateTotalAmount();
                    }
                    else if (productOrder.Quantity == 1)
                    {
                        productOrder.Quantity = 0;
                        UpdateTotalAmount();
                    }
                }
            }
        }
    }

    public class ProductOrder : INotifyPropertyChanged
    {
        private int _quantity;
        private decimal _price;

        public int ProductID { get; set; }
        public string ProductName { get; set; }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged("Quantity");
            }
        }

        public decimal Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
