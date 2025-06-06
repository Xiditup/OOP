﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PhoneService.Converters
{
    public class AntiBoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
            {
                if (val) return Visibility.Collapsed;
                return Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
