using DeliverySystem.Utilites;
using ModelDeliverySystemData.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DeliverySystem.Views.Pages
{
    public partial class UserProfilePage : Page
    {
        private readonly dbContext _context;
        private User _currentUser;
        private bool _isEditMode;

        public UserProfilePage()
        {
            InitializeComponent();
            _context = new dbContext(); // Создание экземпляра контекста базы данных
            LoadUserData();
            SetReadOnlyState(true);
        }

        private void LoadUserData()
        {
            if (App.CurrentUser != null)
            {
                // Загрузка данных текущего пользователя
                _currentUser = _context.Users.FirstOrDefault(u => u.UserID == App.CurrentUser.UserID);

                if (_currentUser != null)
                {
                    FirstNameTextBox.Text = _currentUser.FirstName;
                    LastNameTextBox.Text = _currentUser.LastName;
                    EmailTextBox.Text = _currentUser.Email;
                    PhoneNumberTextBox.Text = _currentUser.PhoneNumber;
                    AccountCreatedTextBox.Text = _currentUser.CreatedAt.ToString();
                    AccountStatusTextBox.Text = _currentUser.IsActive != true ? "Не активен" : "Активен";
                }
            }
        }

        private void SetReadOnlyState(bool isReadOnly)
        {
            FirstNameTextBox.IsReadOnly = isReadOnly;
            LastNameTextBox.IsReadOnly = isReadOnly;
            EmailTextBox.IsReadOnly = isReadOnly;
            PhoneNumberTextBox.IsReadOnly = isReadOnly;
            PasswordTextBox.IsReadOnly = isReadOnly;
            _isEditMode = !isReadOnly;

            EditButton.Content = _isEditMode ? "Сохранить" : "Редактировать";
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode)
            {
                // Сохранение данных пользователя
                if (_currentUser != null && ValidateUserData())
                {
                    _currentUser.FirstName = FirstNameTextBox.Text;
                    _currentUser.LastName = LastNameTextBox.Text;
                    _currentUser.Email = EmailTextBox.Text;
                    _currentUser.PhoneNumber = PhoneNumberTextBox.Text;
                    if (!string.IsNullOrWhiteSpace(PasswordTextBox.Text))
                    {
                        _currentUser.PasswordHash = PasswordHelper.HashPassword(PasswordTextBox.Text);
                    }

                    SaveUserData();
                }

                SetReadOnlyState(true);
            }
            else
            {
                // Переключение в режим редактирования
                SetReadOnlyState(false);
            }
        }

        private bool ValidateUserData()
        {
            // Предположим, что есть методы для валидации, которые возвращают bool
            return !string.IsNullOrWhiteSpace(FirstNameTextBox.Text) &&
                   !string.IsNullOrWhiteSpace(LastNameTextBox.Text) &&
                   !string.IsNullOrWhiteSpace(EmailTextBox.Text) &&
                   !string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text);
        }

        private void SaveUserData()
        {
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode)
            {
                // Отмена изменений и возврат к просмотру
                LoadUserData();
                SetReadOnlyState(true);
            }
            else
            {
                NavigationService.GoBack();
            }
        }
    }
}
