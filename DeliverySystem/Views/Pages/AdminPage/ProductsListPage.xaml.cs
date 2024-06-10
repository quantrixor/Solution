using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ModelDeliverySystemData.Model;

namespace DeliverySystem.Views.Pages.AdminPage
{
    public partial class ProductsListPage : Page
    {
        private dbContext _context;

        public ProductsListPage()
        {
            InitializeComponent();
            _context = new dbContext();
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }
        private void ApplyFilters()
        {
            try
            {
                var filteredProducts = _context.Products.AsQueryable();

                // Фильтрация по текстовому полю
                if (!string.IsNullOrEmpty(SearchBox.Text))
                {
                    filteredProducts = filteredProducts.Where(p => p.ProductName.Contains(SearchBox.Text) ||
                                                                   p.Description.Contains(SearchBox.Text));
                }

                // Фильтрация по дате
                if (StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
                {
                    var startDate = StartDatePicker.SelectedDate.Value.Date;
                    var endDate = EndDatePicker.SelectedDate.Value.Date.AddDays(1).AddTicks(-1);
                    filteredProducts = filteredProducts.Where(p => p.CreateAt >= startDate && p.CreateAt <= endDate);
                }
                else if (StartDatePicker.SelectedDate.HasValue)
                {
                    var startDate = StartDatePicker.SelectedDate.Value.Date;
                    filteredProducts = filteredProducts.Where(p => p.CreateAt >= startDate);
                }
                else if (EndDatePicker.SelectedDate.HasValue)
                {
                    var endDate = EndDatePicker.SelectedDate.Value.Date.AddDays(1).AddTicks(-1);
                    filteredProducts = filteredProducts.Where(p => p.CreateAt <= endDate);
                }

                ProductsListView.ItemsSource = new ObservableCollection<Product>(filteredProducts.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при фильтрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void LoadProducts(string searchQuery = null)
        {
            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                products = products.Where(p => p.ProductName.Contains(searchQuery) || p.Description.Contains(searchQuery));
            }

            ProductsListView.ItemsSource = products.ToList();
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new ManageProductPage());
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {

            var _product = (Product)ProductsListView.SelectedItem;
            if (_product == null || _product.ProductID == 0)
            {
                MessageBox.Show("Продукт не выбран или не существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот продукт и все связанные с ним документы?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаление связанных накладных
                    var invoices = _context.Invoices.Where(i => i.ProductID == _product.ProductID).ToList();
                    _context.Invoices.RemoveRange(invoices);

                    // Удаление связанных документов
                    var documents = _context.ProductDocuments.Where(d => d.ProductID == _product.ProductID).ToList();
                    _context.ProductDocuments.RemoveRange(documents);

                    // Удаление продукта
                    _context.Products.Remove(_product);

                    // Сохранение изменений
                    _context.SaveChanges();

                    MessageBox.Show("Продукт и все связанные документы успешно удалены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при удалении продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListView.SelectedItem is Product selectedProduct)
            {
                NavigationService.Navigate(new ManageProductPage(selectedProduct));
            }
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            SearchBox.Text = string.Empty;
            LoadProducts();
        }
    }
}
