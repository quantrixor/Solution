using System.Windows;
using System.Windows.Controls;

namespace DeliverySystem.UserControls
{
    public partial class PasswordBoxWithToggle : UserControl
    {
        public PasswordBoxWithToggle()
        {
            InitializeComponent();
        }

        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            if (PasswordBoxPassword.Visibility == Visibility.Visible)
            {
                PasswordBoxPassword.Visibility = Visibility.Collapsed;
                TextBoxPassword.Visibility = Visibility.Visible;
                TextBoxPassword.Text = PasswordBoxPassword.Password;
            }
            else
            {
                PasswordBoxPassword.Visibility = Visibility.Visible;
                TextBoxPassword.Visibility = Visibility.Collapsed;
                PasswordBoxPassword.Password = TextBoxPassword.Text;
            }
        }

        public string Password
        {
            get { return PasswordBoxPassword.Password; }
            set
            {
                PasswordBoxPassword.Password = value;
                TextBoxPassword.Text = value;
            }
        }
    }
}
