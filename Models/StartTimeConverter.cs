using System;
using System.Globalization;
using System.Windows.Data;

namespace BidUp_App.Converters
{
    public class StartTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime startTime)
            {
                // Butonul este activ doar dacă StartTime este în trecut
                return DateTime.Now >= startTime;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
