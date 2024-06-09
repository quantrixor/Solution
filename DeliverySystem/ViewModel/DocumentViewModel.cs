namespace DeliverySystem.ViewModel
{
    public class DocumentViewModel
    {
        public string FilePath { get; set; } // Полный путь к файлу
        public string SelectedDocumentType { get; set; }
        public string DisplayName { get; set; } // Имя файла для отображения
        public string DocumentType { get; set; }
        public string Status { get; set; }
    }
}
