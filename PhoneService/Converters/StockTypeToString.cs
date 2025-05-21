using PhoneService.DAL.Models;
using System.Globalization;
using System.Windows.Data;

namespace PhoneService.Converters
{
    public class StockTypeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StockType stockType)
            {
                if (MainWindow.IsRu)
                {
                    return stockType switch
                    {
                        StockType.Discount => "Скидка",
                        StockType.Gift => "Подарок",
                        StockType.NewClient => "Подарок для новых клиентов",
                        StockType.Giveaway => "Розыгрыш",
                        _ => "Скидка"
                    };
                } 
                else
                {
                    return stockType switch
                    {
                        StockType.Discount => "Discount",
                        StockType.Gift => "Gift",
                        StockType.NewClient => "Gift for new clients",
                        StockType.Giveaway => "Giveaway",
                        _ => "Discount"
                    };
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stockStr)
            {
                return value switch
                {
                    "Скидка" => StockType.Discount,
                    "Discount" => StockType.Discount,
                    "Подарок" => StockType.Gift,
                    "Gift" => StockType.Gift,
                    "Подарок для новых клиентов" => StockType.NewClient,
                    "Gift for new clients" => StockType.NewClient,
                    "Розыгрыш" => StockType.Giveaway,
                    "Giveaway" => StockType.Giveaway,
                    _ => StockType.Discount
                };
            }
            return value;
        }
    }
}
