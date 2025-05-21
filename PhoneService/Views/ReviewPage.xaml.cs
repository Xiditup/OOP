using PhoneService.ViewModels;
using System.Windows.Controls;

namespace PhoneService.Views
{
    /// <summary>
    /// Логика взаимодействия для ReviewPage.xaml
    /// </summary>
    public partial class ReviewPage : UserControl
    {
        public ReviewPage(ReviewVM reviewVM)
        {
            InitializeComponent();
            DataContext = reviewVM;
        }
    }
}
