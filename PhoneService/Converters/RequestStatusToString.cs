using PhoneService.DAL.Models;
using System.Globalization;
using System.Windows.Data;

namespace PhoneService.Converters
{
    internal class RequestStatusToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RequestStatus status)
            {
                if (MainWindow.IsRu) {
                    return status switch
                    {
                        RequestStatus.Created => "Создан",
                        RequestStatus.Canceled => "Отменён",
                        RequestStatus.Approved => "Одобрен",
                        RequestStatus.InProgress => "В процессе",
                        RequestStatus.Completed => "Готов к выдаче",
                        RequestStatus.Closed => "Закрыт",
                        _ => "Все",
                    };
                }
                else
                {
                    return status switch
                    {
                        RequestStatus.Created => "Created",
                        RequestStatus.Canceled => "Canceled",
                        RequestStatus.Approved => "Approved",
                        RequestStatus.InProgress => "In progress",
                        RequestStatus.Completed => "Completed",
                        RequestStatus.Closed => "Closed",
                        _ => "All",
                    };
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string statusStr)
            {
                switch (statusStr)
                {
                    case "Создан":
                        return RequestStatus.Created;
                    case "Отменён":
                        return RequestStatus.Canceled;
                    case "Одобрен":
                        return RequestStatus.Approved;
                    case "В процессе":
                        return RequestStatus.InProgress;
                    case "Готов к выдаче":
                        return RequestStatus.Completed;
                    case "Закрыт":
                        return RequestStatus.Closed;
                    case "Все":
                        return RequestStatus.All;
                }
            }
            return value;
        }
    }
}
