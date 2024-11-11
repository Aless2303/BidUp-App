using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BidUp_App.Converters
{
    public class StartTimeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime startTime)
            {
                // Afișează mesajul doar dacă licitația nu a început
                return DateTime.Now < startTime ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
