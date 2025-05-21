using GalaSoft.MvvmLight.Messaging;
using PhoneService.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhoneService.Views
{
    /// <summary>
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>
    public partial class UserPage : UserControl
    {
        public UserPage(UserVM userVM)
        {
            InitializeComponent();
            DataContext = userVM;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is UserVM userVM)
            {
                userVM.ChoosePhoto.Execute(null);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is UserVM userVM)
            {
                userVM.SearchName = sn.Text;
                userVM.SearchLogin = sl.Text;
                userVM.Search.Execute(null);
            }
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            Messenger.Default.Send(false);
            if (DataContext is UserVM userVM)
            {
                userVM.IsEditing = true;
            }
        }

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is UserVM userVM)
            {
                userVM.Search.Execute(null);
            }
        }
    }
}
