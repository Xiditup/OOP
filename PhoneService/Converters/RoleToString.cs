using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PhoneService.Converters
{
    public class RoleToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string role)
            {
                if (MainWindow.IsRu)
                {
                    return role switch
                    {
                        "Client" => "Клиент",
                        "Employee" => "Сотрудник",
                        _ => "Администратор",
                    };
                }
                else
                {
                    return role switch
                    {
                        "Client" => "Client",
                        "Employee" => "Employeee",
                        _ => "Administrator",
                    };
                }
                
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
