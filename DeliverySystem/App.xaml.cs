using DeliverySystem.Model;
using System.Windows;

namespace DeliverySystem
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User CurrentUser { get; set; }
    }
}
