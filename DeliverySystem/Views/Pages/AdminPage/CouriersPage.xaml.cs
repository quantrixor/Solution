using ModelDeliverySystemData.Model;
using DeliverySystem.Views.Windows.AdminWindows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DeliverySystem.Views.Pages.AdminPage
{
    /// <summary>
    /// Логика взаимодействия для CouriersPage.xaml
    /// </summary>
    public partial class CouriersPage : Page
    {
        private dbContext _context;

        public CouriersPage()
        {
            InitializeComponent();
            _context = new dbContext();
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            var listView = sender as ListView;
            var gridView = listView.View as GridView;
            var totalWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;  // Subtract the scrollbar width if visible
            foreach (var column in gridView.Columns)
            {
                column.Width = totalWidth / gridView.Columns.Count;
            }
        }

        private void CouriersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var _selectedCouriesr = (Courier)CouriersListView.SelectedItem;
            if(_selectedCouriesr != null)
            {
                NavigationService.Navigate(new CouriersManagePage(_selectedCouriesr));
            }
        }

        private void LoadDataCouriers()
        {
            var collectionCouriers = _context.Couriers.ToList();
            if(collectionCouriers != null)
            {
                CouriersListView.ItemsSource = collectionCouriers;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataCouriers();
        }

        private void AddCourier_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CouriersManagePage(new Courier()));
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
