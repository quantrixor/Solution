using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModelDeliverySystemData.Model;
using Microsoft.Win32;

namespace DeliverySystem.Views.Pages.AdminPage
{
    public partial class ManageProductPage : Page
    {
        private readonly dbContext _context;
        private readonly Product _product;
        private readonly ObservableCollection<Invoice> _invoices;
        private readonly ObservableCollection<ProductDocument> _documents;

        public ManageProductPage(Product product = null)
        {
            InitializeComponent();
            _context = new dbContext();
            LoadProductTypes();
            _invoices = new ObservableCollection<Invoice>();
            _documents = new ObservableCollection<ProductDocument>();

            // Если продукт передан, значит, это редактирование
            if (product != null)
            {
                _product = product;
                LoadProductData();
            }
            else
            {
                _product = new Product();
            }
        }

        private void LoadProductData()
        {
            txbProductName.Text = _product.ProductName;
            txbDescription.Text = _product.Description;
            txbPrice.Text = _product.Price.ToString("F2");
            txbStockQuantity.Text = _product.StockQuantity.ToString();

            // Установка значения для ComboBox
            if (_product.ProductTypeID.HasValue)
            {
                cmbProductType.SelectedValue = _product.ProductTypeID.Value;
            }
            else
            {
                cmbProductType.SelectedIndex = -1;
            }

            // Установка значений для DateTimePicker
            if (_product.ProductionDate.HasValue)
            {
                dtpProductionDate.Value = _product.ProductionDate.Value;
            }
            else
            {
                dtpProductionDate.Value = null;
            }

            if (_product.ExpiryDate.HasValue)
            {
                dtpExpiryDate.Value = _product.ExpiryDate.Value;
            }
            else
            {
                dtpExpiryDate.Value = null;
            }

            // Проверка наличия накладных
            var invoiceExists = _context.Invoices.Any(i => i.ProductID == _product.ProductID);
            lblInvoiceStatus.Visibility = invoiceExists ? Visibility.Visible : Visibility.Collapsed;
            btnViewInvoice.Visibility = invoiceExists ? Visibility.Visible : Visibility.Collapsed;

            // Проверка наличия документов
            var documentExists = _context.ProductDocuments.Any(d => d.ProductID == _product.ProductID);
            lblDocumentStatus.Visibility = documentExists ? Visibility.Visible : Visibility.Collapsed;
            btnViewDocument.Visibility = documentExists ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadProductTypes()
        {
            var productTypes = _context.ProductTypes.ToList();
            cmbProductType.ItemsSource = productTypes;
            cmbProductType.DisplayMemberPath = "ProductTypeName";
            cmbProductType.SelectedValuePath = "ProductTypeID";
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        private bool ValidateAndSaveProduct()
        {
            // Проверка на пустые значения
            if (string.IsNullOrEmpty(txbProductName.Text) ||
                string.IsNullOrEmpty(txbPrice.Text) ||
                string.IsNullOrEmpty(txbStockQuantity.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверка на корректность числовых значений
            if (!decimal.TryParse(txbPrice.Text, out var price) ||
                !int.TryParse(txbStockQuantity.Text, out var stockQuantity))
            {
                MessageBox.Show("Пожалуйста, введите корректные значения для цены и количества.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            _product.ProductName = txbProductName.Text;
            _product.Description = txbDescription.Text;
            _product.Price = price;
            _product.StockQuantity = stockQuantity;
            _product.ProductTypeID = (int?)cmbProductType.SelectedValue;
            _product.ProductionDate = dtpProductionDate.Value;
            _product.ExpiryDate = dtpExpiryDate.Value;
            _product.CreateAt = DateTime.Now;

            if (_product.ProductID == 0)
            {
                _context.Products.Add(_product);
            }

            _context.SaveChanges();
            return true;
        }

        private void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateAndSaveProduct())
            {
                MessageBox.Show("Продукт успешно сохранен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
        }

        private void UploadInvoice_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, был ли продукт сохранен
            if (_product.ProductID == 0)
            {
                // Сохранение продукта, если он еще не сохранен
                if (!ValidateAndSaveProduct())
                {
                    MessageBox.Show("Ошибка при сохранении продукта. Пожалуйста, попробуйте еще раз.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var openFileDialog = new OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                var invoice = new Invoice
                {
                    ProductID = _product.ProductID,
                    InvoiceDocument = File.ReadAllBytes(filePath),
                    UploadedAt = DateTime.Now
                };

                _context.Invoices.Add(invoice);
                _context.SaveChanges();
                _invoices.Add(invoice);
                MessageBox.Show("Накладная успешно загружена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                lblInvoiceStatus.Visibility = Visibility.Visible;
            }
        }

        private void UploadDocument_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, был ли продукт сохранен
            if (_product.ProductID == 0)
            {
                // Сохранение продукта, если он еще не сохранен
                if (!ValidateAndSaveProduct())
                {
                    MessageBox.Show("Ошибка при сохранении продукта. Пожалуйста, попробуйте еще раз.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var openFileDialog = new OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                var document = new ProductDocument
                {
                    ProductID = _product.ProductID,
                    DocumentType = Path.GetFileNameWithoutExtension(filePath),
                    DocumentContent = File.ReadAllBytes(filePath),
                    UploadedAt = DateTime.Now
                };

                _context.ProductDocuments.Add(document);
                _context.SaveChanges();
                _documents.Add(document);
                MessageBox.Show("Документ успешно загружен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                lblDocumentStatus.Visibility = Visibility.Visible;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ViewInvoice_Click(object sender, RoutedEventArgs e)
        {
            var invoice = _context.Invoices.FirstOrDefault(i => i.ProductID == _product.ProductID);
            if (invoice != null)
            {
                var tempFilePath = Path.GetTempFileName() + ".pdf";
                File.WriteAllBytes(tempFilePath, invoice.InvoiceDocument);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempFilePath) { UseShellExecute = true });
            }
        }

        private void ViewDocument_Click(object sender, RoutedEventArgs e)
        {
            var document = _context.ProductDocuments.FirstOrDefault(d => d.ProductID == _product.ProductID);
            if (document != null)
            {
                var tempFilePath = Path.GetTempFileName() + ".pdf";
                File.WriteAllBytes(tempFilePath, document.DocumentContent);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempFilePath) { UseShellExecute = true });
            }
        }

        private void TxbPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешить ввод только цифр и одной запятой
            if (!char.IsDigit(e.Text, e.Text.Length - 1) && e.Text != ".")
            {
                e.Handled = true;
            }
        }

        private void TxbPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Получить текущий текст
            string text = txbPrice.Text;

            // Если текст пустой или состоит только из одной запятой, выход
            if (string.IsNullOrEmpty(text) || text == ".")
            {
                return;
            }

            // Проверить, правильно ли форматирован текст
            if (!decimal.TryParse(text, out decimal result))
            {
                // Если нет, удалить последнюю введенную часть текста
                txbPrice.Text = text.Substring(0, text.Length - 1);
                txbPrice.CaretIndex = txbPrice.Text.Length;
                return;
            }

            // Проверить, если текст имеет более двух десятичных знаков
            int dotIndex = text.IndexOf('.');
            if (dotIndex >= 0 && text.Length - dotIndex - 1 > 2)
            {
                // Если да, удалить лишние знаки
                txbPrice.Text = text.Substring(0, dotIndex + 3);
                txbPrice.CaretIndex = txbPrice.Text.Length;
            }
        }

        private void TxbPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txbPrice.Text, out decimal result))
            {
                txbPrice.Text = result.ToString("0.00");
            }
        }


    }
}
