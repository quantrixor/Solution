using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeliverySystem.Views.Pages.AdminPage
{
    /// <summary>
    /// Логика взаимодействия для CouriersPage.xaml
    /// </summary>
    public partial class CouriersPage : Page
    {
        public CouriersPage()
        {
            InitializeComponent();
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
    }
}
