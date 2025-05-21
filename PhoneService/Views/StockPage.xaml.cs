using PhoneService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhoneService.Views
{
    /// <summary>
    /// Логика взаимодействия для StockPage.xaml
    /// </summary>
    public partial class StockPage : UserControl
    {
        public StockPage(StockVM stockVM)
        {
            InitializeComponent();
            DataContext = stockVM;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is StockVM stockVM)
            {
                stockVM.ChoosePhoto.Execute(null);
            }
        }
    }
}
