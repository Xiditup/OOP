using PhoneService.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhoneService.Views
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class RegistrationPage : UserControl
    {
        public RegistrationPage(AuthVM authVM)
        {
            InitializeComponent();
            DataContext = authVM;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is AuthVM authVM)
            {
                authVM.ChoosePhoto.Execute(null);
            }
        }
    }
}
