using System;
using System.Globalization;
using System.Windows.Data;

namespace DeliverySystem
{
    public class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double totalWidth = (double)value;
            int columnsCount = System.Convert.ToInt32(parameter);
            if (columnsCount > 0)
                return (totalWidth - 5) / columnsCount;
            return totalWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
