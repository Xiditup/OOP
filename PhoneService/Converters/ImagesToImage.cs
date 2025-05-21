using PhoneService.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PhoneService.Converters
{
    internal class ImagesToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<string> images)
            {
                var mainImage = images.ElementAt(0);
                return new BitmapImage(PhotoBrowser.GetFullPathUri(mainImage));
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
