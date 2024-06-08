using DeliverySystem.Model;
using DeliverySystem.Utilites;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace DeliverySystem.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для SignInWindow.xaml
    /// </summary>
    public partial class SignInWindow : Window
    {
        dbContext _context;
        public SignInWindow()
        {
            InitializeComponent();
            _context = new dbContext();
            LoadUserEmail();
        }
        public void LoadUserEmail()
        {
            // Загрузка сохраненного email и установка его в текстовое поле
            txbUsername.Text = Properties.Settings.Default.LastUserEmail ?? "";
        }

        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            string email = txbUsername.Text;
            string password = psbPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return;
            }

            // Деактивировать кнопку входа во время выполнения операции
            SignIn.IsEnabled = false;
            try
            {
                // Запрос на доступ к системе
                var user = await _context.Users
                                         .FirstOrDefaultAsync(u => u.Email == email);

                if (user != null && PasswordHelper.VerifyPassword(user.PasswordHash, password))
                {
                    App.CurrentUser = user;
                    Properties.Settings.Default.LastUserEmail = email; // Сохранение email пользователя
                    Properties.Settings.Default.Save();
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неправильный логин или пароль.", "В доступе отказано", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                // Показываем сообщение об ошибке, если произошло исключение
                MessageBox.Show("Ошибка при попытке входа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // В любом случае возвращаем кнопке возможность быть нажатой
                SignIn.IsEnabled = true;
            }
        }


        private void RequestAccess_Click(object sender, RoutedEventArgs e)
        {
            RequestAccessWindow requestAccessWindow = new RequestAccessWindow();
            requestAccessWindow.ShowDialog();
        }
    }
}
