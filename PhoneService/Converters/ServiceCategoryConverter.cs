using PhoneService.DAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PhoneService.Converters
{
    public class ServiceCategoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ServiceCategory serviceCategory)
            {
                if (MainWindow.IsRu)
                {
                    return serviceCategory switch
                    {
                        ServiceCategory.Repair => "Починка",
                        ServiceCategory.Replacement => "Замена",
                        ServiceCategory.Maintenance => "Обслуживание",
                        ServiceCategory.Diagnostics => "Диагностика",
                        ServiceCategory.Recovery => "Восстановление",
                        ServiceCategory.Update => "Обновление",
                        ServiceCategory.All => "Все",
                        _ => "Разблокировка"
                    };
                }
                else
                {
                    return serviceCategory switch
                    {
                        ServiceCategory.Repair => "Repair",
                        ServiceCategory.Replacement => "Replacement",
                        ServiceCategory.Maintenance => "Maintenance",
                        ServiceCategory.Diagnostics => "Diagnostics",
                        ServiceCategory.Recovery => "Recovery",
                        ServiceCategory.Update => "Update",
                        ServiceCategory.All => "All",
                        _ => "Unlock"
                    };
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string categoryStr)
            {
                if (MainWindow.IsRu)
                {
                    return categoryStr switch
                    {
                        "Починка" => ServiceCategory.Repair,
                        "Замена" => ServiceCategory.Replacement,
                        "Обслуживание" => ServiceCategory.Maintenance,
                        "Диагностика" => ServiceCategory.Diagnostics,
                        "Восстановление" => ServiceCategory.Recovery,
                        "Обновление" => ServiceCategory.Update,
                        "Все" => ServiceCategory.All,
                        _ => ServiceCategory.Unlocking
                    };
                }
                else
                {
                    return categoryStr switch
                    {
                        "Repair" => ServiceCategory.Repair,
                        "Replacement" => ServiceCategory.Replacement,
                        "Maintenance" => ServiceCategory.Maintenance,
                        "Diagnostics" => ServiceCategory.Diagnostics,
                        "Recovery" => ServiceCategory.Recovery,
                        "Update" => ServiceCategory.Update,
                        "All" => ServiceCategory.All,
                        _ => ServiceCategory.Unlocking
                    };
                }
            }
            return value;
        }
    }
}
