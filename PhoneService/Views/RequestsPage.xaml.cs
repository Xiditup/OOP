using PhoneService.DAL.Entities;
using PhoneService.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhoneService.Views
{
    /// <summary>
    /// Логика взаимодействия для ActiveRequestsPage.xaml
    /// </summary>
    public partial class RequestsPage : UserControl
    {
        public RequestsPage(RequestsVM requestsVM)
        {
            InitializeComponent();
            DataContext = requestsVM;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.DataContext is Request request)
            {
                if (DataContext is RequestsVM requestsVM)
                {
                    requestsVM.OpenRequest.Execute(request);
                }
            }
        }
    }
}
