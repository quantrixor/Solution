using DeliverySystem.Views.Pages.AdminPage;
using DeliverySystem.Views.Windows;
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
    }
}
