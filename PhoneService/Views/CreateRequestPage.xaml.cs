using PhoneService.ViewModels;
using System.Windows.Controls;

namespace PhoneService.Views
{
    /// <summary>
    /// Логика взаимодействия для RequestPage.xaml
    /// </summary>
    public partial class CreateRequestPage : UserControl
    {
        public CreateRequestPage(CreateRequestVM requestVM)
        {
            InitializeComponent();
            DataContext = requestVM;
        }
    }
}
