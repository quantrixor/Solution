using DeliverySystem.Model;
using DeliverySystem.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DeliverySystem.Views.Windows.AdminWindows
{
    public partial class CourierDocumentsWindow : Window
    {
        private ObservableCollection<DocumentViewModel> selectedFiles;
        private Courier _courier;
        private dbContext _context;

        public CourierDocumentsWindow(Courier courier)
        {
            InitializeComponent();
            selectedFiles = new ObservableCollection<DocumentViewModel>();
            _courier = courier;
            _context = new dbContext();

            // Подписываемся на событие открытия выпадающего списка
            cbDocumentType.DropDownOpened += OnComboBoxOpened;
            LoadExistingDocuments();
        }

        private void OnComboBoxOpened(object sender, EventArgs e)
        {
            UpdateComboBoxItems();
        }

        private void UpdateComboBoxItems()
        {
            // Загружаем данные документов в память
            var documents = _context.CourierDocuments
                .Where(d => d.CourierID == _courier.CourierID)
                .ToList();

            // Создаем список загруженных типов документов
            var loadedTypes = new List<string>();
            foreach (var doc in documents)
            {
                if (doc.Passport != null)
                    loadedTypes.Add("Паспорт");
                if (doc.INN != null)
                    loadedTypes.Add("ИНН");
                if (doc.SNILS != null)
                    loadedTypes.Add("СНИЛС");
                if (doc.DriverLicense != null)
                    loadedTypes.Add("Водительское удостоверение");
                if (doc.ContractCopy != null)
                    loadedTypes.Add("Копия контракта");
                if (doc.BankDetails != null)
                    loadedTypes.Add("Банковские реквизиты");
            }

            // Полный список всех типов документов
            var allDocumentTypes = new List<string>
            {
                "Паспорт",
                "ИНН",
                "СНИЛС",
                "Водительское удостоверение",
                "Копия контракта",
                "Банковские реквизиты"
            };

            // Очищаем текущие элементы ComboBox
            cbDocumentType.Items.Clear();

            // Добавляем только те типы, которые еще не загружены
            foreach (var docType in allDocumentTypes)
            {
                if (!loadedTypes.Contains(docType))
                {
                    cbDocumentType.Items.Add(new ComboBoxItem { Content = docType });
                }
            }
        }

        private void LoadExistingDocuments()
        {
            selectedFiles.Clear();
            var documents = _context.CourierDocuments.Where(d => d.CourierID == _courier.CourierID);

            foreach (var doc in documents)
            {
                AddDocumentToView(doc, "Паспорт", doc.Passport);
                AddDocumentToView(doc, "ИНН", doc.INN);
                AddDocumentToView(doc, "СНИЛС", doc.SNILS);
                AddDocumentToView(doc, "Водительское удостоверение", doc.DriverLicense);
                AddDocumentToView(doc, "Копия контракта", doc.ContractCopy);
                AddDocumentToView(doc, "Банковские реквизиты", doc.BankDetails);
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

        private void UploadDocument_Click(object sender, RoutedEventArgs e)
        {
            string selectedDocumentType = (cbDocumentType.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(selectedDocumentType))
            {
                MessageBox.Show("Please select the type of document.", "Unknown Type", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "PDF Files (*.pdf)|*.pdf"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"File {filePath} not found.");
                    return;
                }

                long fileSize = new FileInfo(filePath).Length;
                if (fileSize > 10485760) // 10 MB limit
                {
                    MessageBox.Show("The file is too large.", "Limit Exceeded", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                byte[] fileData = File.ReadAllBytes(filePath);

                // Получаем или создаем документ для курьера
                var document = _context.CourierDocuments.FirstOrDefault(d => d.CourierID == _courier.CourierID) ?? new CourierDocument { CourierID = _courier.CourierID };

                // Если это новая запись, добавляем в контекст
                if (document.DocumentID == 0)
                {
                    _context.CourierDocuments.Add(document);
                }

                // Установка свойства в зависимости от выбранного типа документа
                SetProperty(document, selectedDocumentType, fileData);

                try
                {
                    _context.SaveChanges();
                    MessageBox.Show("Document successfully uploaded.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadExistingDocuments();
                    UpdateComboBoxItems();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save the document: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Очищаем выбор в ComboBox
                cbDocumentType.SelectedIndex = -1;
            }
        }

        private void SetProperty(CourierDocument document, string documentType, byte[] fileData)
        {
            switch (documentType)
            {
                case "Паспорт":
                    document.Passport = fileData;
                    break;
                case "ИНН":
                    document.INN = fileData;
                    break;
                case "СНИЛС":
                    document.SNILS = fileData;
                    break;
                case "Водительское удостоверение":
                    document.DriverLicense = fileData;
                    break;
                case "Копия контракта":
                    document.ContractCopy = fileData;
                    break;
                case "Банковские реквизиты":
                    document.BankDetails = fileData;
                    break;
            }
        }

        private void DeleteDocument_Click(object sender, RoutedEventArgs e)
        {
            var selectedDocumentViewModel = lvDocuments.SelectedItem as DocumentViewModel;
            if (selectedDocumentViewModel == null)
            {
                MessageBox.Show("Please select a document to delete.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var documentType = selectedDocumentViewModel.SelectedDocumentType;

            var document = _context.CourierDocuments.FirstOrDefault(d => d.CourierID == _courier.CourierID);
            if (document == null)
            {
                MessageBox.Show("Document not found in the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Очистка поля документа
            ClearDocumentProperty(document, documentType);

            try
            {
                _context.SaveChanges();
                MessageBox.Show("Document has been deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновляем список документов и доступные типы в ComboBox
                LoadExistingDocuments();
                UpdateComboBoxItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting the document: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearDocumentProperty(CourierDocument document, string documentType)
        {
            switch (documentType)
            {
                case "Паспорт":
                    document.Passport = null;
                    break;
                case "ИНН":
                    document.INN = null;
                    break;
                case "СНИЛС":
                    document.SNILS = null;
                    break;
                case "Водительское удостоверение":
                    document.DriverLicense = null;
                    break;
                case "Копия контракта":
                    document.ContractCopy = null;
                    break;
                case "Банковские реквизиты":
                    document.BankDetails = null;
                    break;
            }
        }

    }
}
