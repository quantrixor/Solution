using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using ModelDeliverySystemData.Model;
using System;
using System.Collections.ObjectModel;

namespace DeliverySystem.Views.Pages.AdminPage
{
    public partial class ClientsPage : Page
    {
        private dbContext _context;
        public ObservableCollection<Client> Clients { get; set; }
        public List<Region> Regions { get; set; }
        public List<City> Cities { get; set; }

        public ClientsPage()
        {
            InitializeComponent();
            _context = new dbContext();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Initial load of clients and regions
            LoadClients();
            LoadRegions();
        }

        private void LoadClients()
        {
            // Загрузка клиентов из базы данных
            var clients = _context.Clients.Include("Regions").Include("Cities").ToList();
            Clients = new ObservableCollection<Client>(clients);
            ClientsListView.ItemsSource = Clients;
        }

        private void LoadRegions()
        {
            // Load regions from database
            Regions = _context.Regions.ToList();
            RegionComboBox.ItemsSource = Regions;
        }

        private void LoadCities(int regionId)
        {
            CityComboBox.ItemsSource = _context.Cities.Where(c => c.RegionID == regionId).ToList();
            CityComboBox.SelectedIndex = -1;  // Сброс выбранного города при смене региона
        }


        private void FilterClients()
        {
            IEnumerable<Client> filteredClients = Clients;

            if (RegionComboBox.SelectedItem is Region selectedRegion)
            {
                filteredClients = filteredClients.Where(c => c.RegionID == selectedRegion.RegionID);
            }

            if (CityComboBox.SelectedItem is City selectedCity)
            {
                filteredClients = filteredClients.Where(c => c.CityID == selectedCity.CityID);
            }

            if (!string.IsNullOrEmpty(SearchBox.Text))
            {
                filteredClients = filteredClients.Where(c =>
                    c.FirstName.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    c.LastName.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    c.Email.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    c.PhoneNumber.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            ClientsListView.ItemsSource = filteredClients.ToList();
        }

        private void RegionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RegionComboBox.SelectedItem is Region selectedRegion)
            {
                LoadCities(selectedRegion.RegionID);
            }
            else
            {
                CityComboBox.ItemsSource = null;
            }
            FilterClients();
        }

        private void CityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterClients();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterClients();
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            RegionComboBox.SelectedItem = null;
            CityComboBox.ItemsSource = null;
            SearchBox.Text = string.Empty;
            ClientsListView.ItemsSource = Clients;
        }



        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManageClientPage(0));
        }

        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            var _selectedClient = ClientsListView.SelectedItem as Client;
            if (_selectedClient != null)
            {
                NavigationService.Navigate(new ManageClientPage(_selectedClient.ClientID));
            }
        }


        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = ClientsListView.SelectedItem as Client;
            if (selectedClient == null)
            {
                MessageBox.Show("Пожалуйста, выберите клиента для удаления.");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить выбранного клиента?",
                                         "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Clients.Remove(selectedClient);
                    _context.SaveChanges();
                    Clients.Remove(selectedClient);  // Удаление из ObservableCollection автоматически обновит ListView
                    MessageBox.Show("Клиент был успешно удален.", "Удалено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении клиента: " + ex.Message);
                }
            }
        }
    }
}
