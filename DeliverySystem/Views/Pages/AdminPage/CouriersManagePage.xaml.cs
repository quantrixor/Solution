using DeliverySystem.Model;
using DeliverySystem.ViewModel;
using DeliverySystem.Views.Windows.AdminWindows;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DeliverySystem.Views.Pages.AdminPage
{
    /// <summary>
    /// Логика взаимодействия для CouriersManagePage.xaml
    /// </summary>
    public partial class CouriersManagePage : Page
    {
        private Courier _courier;
        private dbContext _context;
        private ObservableCollection<DocumentViewModel> selectedFiles;
        private bool isEditMode = false;

        public CouriersManagePage(Courier courier)
        {
            InitializeComponent();
            _courier = courier;
            _context = new dbContext();
            selectedFiles = new ObservableCollection<DocumentViewModel>();
            LoadExistingDocuments();
            LoadCourierData();
        }

        private void LoadCourierData()
        {
            if (_courier != null)
            {
                txbFirstName.Text = _courier.FirstName;
                txbLastName.Text = _courier.LastName;
                txbLicenseNumber.Text = _courier.LicenseNumber;
                txbPhoneNumber.Text = _courier.PhoneNumber;
                txbVehicleID.Text = _courier.VehicleID != null ? _courier.VehicleID.ToString() : "ТС не добавлено";
            }
        }

        private void SetReadOnlyMode(bool isReadOnly)
        {
            txbFirstName.IsReadOnly = isReadOnly;
            txbLastName.IsReadOnly = isReadOnly;
            txbLicenseNumber.IsReadOnly = isReadOnly;
            txbPhoneNumber.IsReadOnly = isReadOnly;
            //txbVehicleID.IsReadOnly = isReadOnly;
        }
        private void SaveCourier_Click(object sender, RoutedEventArgs e)
        {
            if (isEditMode)
            {
                try
                {
                    if (_courier.CourierID == 0)
                    {
                        // Добавление нового курьера
                        _courier = new Courier
                        {
                            FirstName = txbFirstName.Text,
                            LastName = txbLastName.Text,
                            LicenseNumber = txbLicenseNumber.Text,
                            PhoneNumber = txbPhoneNumber.Text,
                        };
                        _context.Couriers.Add(_courier);
                    }
                    else
                    {
                        // Обновление существующего курьера
                        var existingCourier = _context.Couriers.FirstOrDefault(c => c.CourierID == _courier.CourierID);
                        if (existingCourier != null)
                        {
                            existingCourier.FirstName = txbFirstName.Text;
                            existingCourier.LastName = txbLastName.Text;
                            existingCourier.LicenseNumber = txbLicenseNumber.Text;
                            existingCourier.PhoneNumber = txbPhoneNumber.Text;
                            existingCourier.VehicleID = int.Parse(txbVehicleID.Text);
                        }
                    }
                    _context.SaveChanges();
                    MessageBox.Show("Данные курьера успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    SetReadOnlyMode(true);
                    SaveCourier.Content = "Редактировать";
                    isEditMode = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при сохранении данных курьера: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                SetReadOnlyMode(false);
                SaveCourier.Content = "Сохранить";
                isEditMode = true;
            }
        }

        private void PhoneNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                return;
            }

            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
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

        private void LicenseNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверка на ввод только цифр
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                return;
            }

            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            string text = textBox.Text + e.Text;

            // Форматируем текст по мере ввода
            if (text.Length == 2 || text.Length == 5)
            {
                textBox.Text += " ";
                textBox.SelectionStart = textBox.Text.Length;
            }

            // Ограничиваем длину текста по маске
            if (textBox.Text.Length >= 13)
            {
                e.Handled = true;
            }
        }

        private void LoadExistingDocuments()
        {
            selectedFiles.Clear();
            var documents = _context.CourierDocuments.Where(d => d.CourierID == _courier.CourierID).ToList();

            foreach (var doc in documents)
            {
                if (doc.Passport != null) AddDocumentToView(doc, "Паспорт", doc.Passport);
                if (doc.INN != null) AddDocumentToView(doc, "ИНН", doc.INN);
                if (doc.SNILS != null) AddDocumentToView(doc, "СНИЛС", doc.SNILS);
                if (doc.DriverLicense != null) AddDocumentToView(doc, "Водительское удостоверение", doc.DriverLicense);
                if (doc.ContractCopy != null) AddDocumentToView(doc, "Копия контракта", doc.ContractCopy);
                if (doc.BankDetails != null) AddDocumentToView(doc, "Банковские реквизиты", doc.BankDetails);
            }

            lvDocuments.ItemsSource = selectedFiles;
        }

        private void AddDocumentToView(CourierDocument doc, string documentType, byte[] documentContent)
        {
            if (documentContent != null)
            {
                selectedFiles.Add(new DocumentViewModel
                {
                    SelectedDocumentType = documentType,
                    DisplayName = $"{documentType}.pdf",
                    FilePath = "Загружено"
                });
            }
        }

        private void DownloadDocuments_Click(object sender, RoutedEventArgs e)
        {
            var selectedDocuments = lvDocuments.SelectedItems.Cast<DocumentViewModel>().ToList();
            if (selectedDocuments.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите один или несколько документов для скачивания.", "Ничего не выбрано", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "ZIP files (*.zip)|*.zip",
                Title = "Сохранить документы в ZIP"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (ZipArchive archive = ZipFile.Open(saveFileDialog.FileName, ZipArchiveMode.Create))
                    {
                        bool anyDocumentsAdded = false;

                        foreach (var doc in selectedDocuments)
                        {
                            Console.WriteLine($"Searching for document type: {doc.SelectedDocumentType} for courier ID: {_courier.CourierID}");
                            var document = _context.CourierDocuments.FirstOrDefault(d => d.CourierID == _courier.CourierID);
                            if (document == null)
                            {
                                MessageBox.Show($"Документ типа '{doc.SelectedDocumentType}' не найден для курьера с ID {_courier.CourierID}.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                Console.WriteLine($"No document found for courier ID {_courier.CourierID}");
                                continue;
                            }

                            var content = GetDocumentContent(document, doc.SelectedDocumentType);
                            if (content != null && content.Length > 0)
                            {
                                var entry = archive.CreateEntry($"{doc.SelectedDocumentType}.pdf");
                                using (var entryStream = entry.Open())
                                {
                                    entryStream.Write(content, 0, content.Length);
                                }
                                anyDocumentsAdded = true;
                            }
                            else
                            {
                                MessageBox.Show($"Содержимое документа '{doc.SelectedDocumentType}' отсутствует или повреждено.", "Проблема с содержимым", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }

                        if (!anyDocumentsAdded)
                        {
                            MessageBox.Show("Ни один из выбранных документов не был добавлен в архив.", "Нет документов", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Документы успешно скачаны.", "Загрузка завершена", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при скачивании документов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private byte[] GetDocumentContent(CourierDocument document, string documentType)
        {
            if (document == null) return null;

            switch (documentType)
            {
                case "Паспорт":
                    return document.Passport;
                case "ИНН":
                    return document.INN;
                case "СНИЛС":
                    return document.SNILS;
                case "Водительское удостоверение":
                    return document.DriverLicense;
                case "Копия контракта":
                    return document.ContractCopy;
                case "Банковские реквизиты":
                    return document.BankDetails;
                default:
                    return null; // Возвращаем null, если тип документа не распознан
            }
        }

        private void ManageDocuments_Click(object sender, RoutedEventArgs e)
        {
            if(_courier != null)
            {
                CourierDocumentsWindow courierDocumentsWindow = new CourierDocumentsWindow(_courier);
                courierDocumentsWindow.ShowDialog();
            }
        }

        private void VehiclesCourier_Click(object sender, RoutedEventArgs e)
        {
            if(_courier != null)
            {
                NavigationService.Navigate(new VehicleManagePage(_courier));
            }
        }

        private int? GetVehicleID()
        {
            if (_courier == null) return null;

            var vehicle = _context.Vehicles.FirstOrDefault(v => v.VehicleID == _courier.VehicleID);
            return vehicle?.VehicleID;
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int? vehicleID = GetVehicleID();
            if (vehicleID != null)
            {
                txbVehicleID.Text = vehicleID.ToString();
            }
            else
            {
                txbVehicleID.Text = "ТС не добавлено";
            }
        }

    }
}
