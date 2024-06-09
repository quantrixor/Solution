using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DeliverySystem.Model;

namespace DeliverySystem.Views.Pages.AdminPage
{
    public partial class ProductsListPage : Page
    {
        private dbContext _context;
        private Product _selectedProduct;

        public ProductsListPage()
        {
            InitializeComponent();
            _context = new dbContext();
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

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadProducts(SearchBox.Text);
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new ManageProductPage());
        }

        private void ProductsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductsListView.SelectedItem is Product selectedProduct)
            {
                NavigationService.Navigate(new ManageProductPage(selectedProduct));
            }
        }
    }
}
