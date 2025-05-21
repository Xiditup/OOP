using PhoneService.ViewModels;
using System.Windows.Controls;

namespace PhoneService.Views
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class AuthorizationPage : UserControl
    {
        public AuthorizationPage(AuthVM authVM)
        {
            InitializeComponent();
            DataContext = authVM;
        }
    }
}
