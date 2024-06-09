using DeliverySystem.Views.Pages.AdminPage;
using DeliverySystem.Views.Windows;
using System;
using System.Windows;

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
    }
}
