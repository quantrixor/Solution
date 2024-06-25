using ModelDeliverySystemData.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DeliverySystem.Views.Pages.AdminPage
{
    /// <summary>
    /// Логика взаимодействия для ManageClientPage.xaml
    /// </summary>
    public partial class ManageClientPage : Page
    {
        private dbContext _context;
        private Client _client;
        
        public ManageClientPage(int? clientId)
        {
            InitializeComponent();
            _context = new dbContext();
            LoadRegions();

            if (clientId.HasValue && clientId > 0)
            {
                // Загрузка существующего клиента
                _client = _context.Clients.FirstOrDefault(c => c.ClientID == clientId);
                if (_client != null)
                {
                    LoadClientData();
                    if (_client.RegionID.HasValue)
                    {
                        LoadCities(_client.RegionID.Value);
                    }
                }
            }
            else
            {
                // Создание нового клиента
                _client = new Client();
            }
        }

        private void CmbRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRegion.SelectedValue != null)
            {
                int regionId = (int)cmbRegion.SelectedValue;
                LoadCities(regionId);
            }
            else
            {
                cmbCity.ItemsSource = null;
            }
        }
        
        private void LoadRegions()
        {
            try
            {
                var regions = _context.Regions.ToList();
                cmbRegion.ItemsSource = regions;
                cmbRegion.DisplayMemberPath = "RegionName";
                cmbRegion.SelectedValuePath = "RegionID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки регионов: " + ex.Message);
            }
        }
       
        private void LoadClientData()
        {
            if (_client != null)
            {
                txbFirstName.Text = _client.FirstName;
                txbLastName.Text = _client.LastName;
                txbMiddleName.Text = _client.MiddleName;
                txbEmail.Text = _client.Email;
                txbPhoneNumber.Text = _client.PhoneNumber;
                cmbRegion.SelectedValue = _client.RegionID;
                cmbCity.SelectedValue = _client.CityID;
                txbStreetAddress.Text = _client.StreetAddress;
            }
        }

        private void LoadCities(int regionId)
        {
            try
            {
                var cities = _context.Cities.Where(c => c.RegionID == regionId).ToList();
                cmbCity.ItemsSource = cities;
                cmbCity.DisplayMemberPath = "CityName";
                cmbCity.SelectedValuePath = "CityID";

                if (_client != null && _client.CityID.HasValue)
                {
                    cmbCity.SelectedValue = _client.CityID;
                }
                else
                {
                    cmbCity.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки городов: " + ex.Message);
            }
        }

        private void SaveClient_Click(object sender, RoutedEventArgs e)
        {
            // Валидация введенных данных
            if (string.IsNullOrWhiteSpace(txbFirstName.Text) ||
                string.IsNullOrWhiteSpace(txbLastName.Text) ||
                string.IsNullOrWhiteSpace(txbEmail.Text) ||
                string.IsNullOrWhiteSpace(txbPhoneNumber.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _client.FirstName = txbFirstName.Text;
                _client.LastName = txbLastName.Text;
                _client.MiddleName = txbMiddleName.Text;
                _client.Email = txbEmail.Text;
                _client.PhoneNumber = txbPhoneNumber.Text;
                _client.RegionID = (int?)cmbRegion.SelectedValue;
                _client.CityID = (int?)cmbCity.SelectedValue;
                _client.StreetAddress = txbStreetAddress.Text;

                if (_client.ClientID == 0)
                {
                    _context.Clients.Add(_client); // Добавление нового клиента, если это новый клиент
                }
                else
                {
                    if (_context.Entry(_client).State == EntityState.Detached)
                    {
                        _context.Clients.Attach(_client);
                    }
                    _context.Entry(_client).State = EntityState.Modified;
                }

                // Сохраняем изменения используя тот же контекст
                _context.SaveChanges();
                MessageBox.Show("Данные клиента успешно сохранены.", "Всё прошло успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении клиента: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
            GC.Collect();
        }
    }
}
