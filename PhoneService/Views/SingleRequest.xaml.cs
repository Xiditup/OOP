using PhoneService.ViewModels;
using System.Windows.Controls;

namespace PhoneService.Views
{
    /// <summary>
    /// Логика взаимодействия для SingleRequest.xaml
    /// </summary>
    public partial class SingleRequestPage : UserControl
    {
        public SingleRequestPage(SingleRequestVM singleRequestVM)
        {
            InitializeComponent();
            DataContext = singleRequestVM;
        }
    }
}
