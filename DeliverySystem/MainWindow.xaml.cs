using DeliverySystem.Views.Pages;
using DeliverySystem.Views.Pages.AdminPage;
using DeliverySystem.Views.Windows;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace DeliverySystem
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new ProductsListPage());
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            SignInWindow signInWindow = new SignInWindow();
            signInWindow.Show();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtCurrentUser.Text = $"{App.CurrentUser.FirstName} {App.CurrentUser.LastName}";
            lblEmail.Content = App.CurrentUser.Email;
            lblCurrentDate.Content = DateTime.Now.ToString("D");
        }

        private void CouriersList_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CouriersPage());
        }

        private void Product_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductsListPage());
        }

        private void Oreders_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrdersPage());
        }

        private void EditDataCurrentUser_Click(object sender, RoutedEventArgs e)
        {
            if(App.CurrentUser != null)
            {
                MainFrame.Navigate(new UserProfilePage());  
            }
        }

        private void ClientsList_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientsPage());
        }

        private void SalesStatisticsPage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SalesStatisticsPage());
        }
    }
}
