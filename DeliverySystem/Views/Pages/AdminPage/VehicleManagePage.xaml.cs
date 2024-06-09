using ModelDeliverySystemData.Model;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DeliverySystem.Views.Pages.AdminPage
{
    public partial class VehicleManagePage : Page
    {

        private Courier _courier;
        private dbContext _context;
        private Vehicle _vehicle;

        public VehicleManagePage(Courier courier)
        {
            InitializeComponent();
            _courier = courier;
            _context = new dbContext();
            LoadComboBoxes();
            LoadVehicleData();
        }

        private void LoadVehicleData()
        {
            _vehicle = _context.Vehicles.FirstOrDefault(v => v.VehicleID == _courier.VehicleID);
            if (_vehicle != null)
            {
                txbLicensePlate.Text = _vehicle.LicensePlate;
                var model = _context.Models.FirstOrDefault(m => m.ModelID == _vehicle.ModelID);
                if (model != null)
                {
                    cmbBrand.SelectedValue = model.BrandID;
                    LoadModels(model.BrandID);
                    cmbModel.SelectedValue = model.ModelID;
                }
                cmbType.SelectedValue = _vehicle.TypeID;
                // Управление видимостью кнопок
                btnViewInsuranceDocument.Visibility = _vehicle.InsuranceDocument != null ? Visibility.Visible : Visibility.Collapsed;
                btnViewTechnicalPassport.Visibility = _vehicle.TechnicalPassport != null ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                // Если транспортное средство не найдено, скрываем кнопки
                btnViewInsuranceDocument.Visibility = Visibility.Collapsed;
                btnViewTechnicalPassport.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadComboBoxes()
        {
            var brands = _context.Brands.ToList();
            cmbBrand.ItemsSource = brands;
            cmbBrand.DisplayMemberPath = "BrandName";
            cmbBrand.SelectedValuePath = "BrandID";

            var types = _context.VehicleTypes.ToList();
            cmbType.ItemsSource = types;
            cmbType.DisplayMemberPath = "TypeName";
            cmbType.SelectedValuePath = "TypeID";
        }

        private void LoadModels(int brandID)
        {
            var models = _context.Models.Where(m => m.BrandID == brandID).ToList();
            cmbModel.ItemsSource = models;
            cmbModel.DisplayMemberPath = "ModelName";
            cmbModel.SelectedValuePath = "ModelID";
        }

        private void cmbBrand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBrand.SelectedValue != null)
            {
                int brandID = (int)cmbBrand.SelectedValue;
                LoadModels(brandID);
            }
        }


        private void UploadInsuranceDocument_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var documentContent = System.IO.File.ReadAllBytes(openFileDialog.FileName);
                if (_vehicle != null)
                {
                    _vehicle.InsuranceDocument = documentContent;
                    _context.SaveChanges();
                    btnViewInsuranceDocument.Visibility = _vehicle.InsuranceDocument != null ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Сначала сохраните транспортное средство.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void UploadTechnicalPassport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var documentContent = System.IO.File.ReadAllBytes(openFileDialog.FileName);
                if (_vehicle != null)
                {
                    _vehicle.TechnicalPassport = documentContent;
                    _context.SaveChanges();
                    btnViewTechnicalPassport.Visibility = _vehicle.TechnicalPassport != null ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Сначала сохраните транспортное средство.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void SaveVehicle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка существования транспортного средства
                if (_courier.VehicleID != null)
                {
                    _vehicle = _context.Vehicles.FirstOrDefault(v => v.VehicleID == _courier.VehicleID);
                }

                // Если транспортное средство не существует, создаем новое
                if (_vehicle == null)
                {
                    _vehicle = new Vehicle
                    {
                        LicensePlate = txbLicensePlate.Text,
                        ModelID = (int)cmbModel.SelectedValue,
                        TypeID = (int)cmbType.SelectedValue
                    };
                    _context.Vehicles.Add(_vehicle);
                    _context.SaveChanges();

                    _courier.VehicleID = _vehicle.VehicleID;
                    _context.SaveChanges();
                }
                else
                {
                    // Обновляем существующее транспортное средство
                    _vehicle.LicensePlate = txbLicensePlate.Text;
                    _vehicle.ModelID = (int)cmbModel.SelectedValue;
                    _vehicle.TypeID = (int)cmbType.SelectedValue;
                    _context.SaveChanges();
                }

                MessageBox.Show("Данные транспортного средства успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении данных транспортного средства: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ViewInsuranceDocument_Click(object sender, RoutedEventArgs e)
        {
            if (_vehicle?.InsuranceDocument != null)
            {
                OpenDocument(_vehicle.InsuranceDocument, "InsuranceDocument.pdf");
            }
            else
            {
                MessageBox.Show("Страховой документ не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewTechnicalPassport_Click(object sender, RoutedEventArgs e)
        {
            if (_vehicle?.TechnicalPassport != null)
            {
                OpenDocument(_vehicle.TechnicalPassport, "TechnicalPassport.pdf");
            }
            else
            {
                MessageBox.Show("Технический паспорт не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenDocument(byte[] documentContent, string fileName)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), fileName);
            File.WriteAllBytes(tempFilePath, documentContent);
            Process.Start(new ProcessStartInfo(tempFilePath) { UseShellExecute = true });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
