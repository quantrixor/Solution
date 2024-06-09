using ModelDeliverySystemData.Model;
using DeliverySystem.Utilites;
using DeliverySystem.Validators;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DeliverySystem.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для RequestAccessWindow.xaml
    /// </summary>
    public partial class RequestAccessWindow : Window
    {
        public RequestAccessWindow()
        {
            InitializeComponent();
        }
        private void PhoneNumberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                return;
            }

            TextBox textBox = sender as TextBox;
            string text = textBox.Text + e.Text;

            if (text.Length == 1)
            {
                textBox.Text = "+7 (";
                textBox.SelectionStart = textBox.Text.Length;
            }
            else if (text.Length == 8)
            {
                textBox.Text += ") ";
                textBox.SelectionStart = textBox.Text.Length;
            }
            else if (text.Length == 13 || text.Length == 16)
            {
                textBox.Text += "-";
                textBox.SelectionStart = textBox.Text.Length;
            }

            if (textBox.Text.Length >= 18)
            {
                e.Handled = true;
            }
        }

        private async void RequestAccess_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!EmailValidator.IsValidEmail(txbEmail.Text))
                {
                    MessageBox.Show("Введенные вами данные электронной почты не действительны.",
                        "E-mail отклонён", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!EmailValidator.CheckUniqueEmail(txbEmail.Text))
                {
                    MessageBox.Show("Пользователь с таким адресом электронной почты уже существует в системе.",
                        "E-mail отклонён", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Генерация пароля
                string generatedPassword = PasswordGenerator.GeneratePassword();
                string hashedPassword = PasswordHelper.HashPassword(generatedPassword);

                // Подготовка письма
                string emailBody = $"Здравствуйте, {txbFirstName.Text} {txbLastName.Text}. Ваш временный пароль: {generatedPassword}. Пожалуйста, измените пароль после входа в систему.";

                using (var _context = new dbContext())
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            // Создание нового пользователя
                            User user = new User
                            {
                                FirstName = txbFirstName.Text,
                                LastName = txbLastName.Text,
                                Email = txbEmail.Text,
                                PhoneNumber = txbPhoneNumber.Text,
                                PasswordHash = hashedPassword,
                                IsActive = false,
                                RoleID = 2, // По умолчанию 'Пользователь'
                                CreatedAt = DateTime.Now
                            };

                            _context.Users.Add(user);
                            await _context.SaveChangesAsync();

                            // Попытка отправить письмо
                            await NotifyClass.SendEmailAsync(txbEmail.Text, "Добро пожаловать в систему!", emailBody);

                            // Если письмо отправлено успешно, подтверждаем транзакцию
                            transaction.Commit();
                            MessageBox.Show("Данные успешно сохранены в базу данных, вам на почту было отправлено письмо с приглашением в систему.",
                                "Данные успешно сохранены", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close();
                        }
                        catch (Exception)
                        {
                            // Если ошибка при отправке, откатываем транзакцию
                            transaction.Rollback();
                            throw; // Переброс исключения для обработки в следующем блоке catch
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}",
                    "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

