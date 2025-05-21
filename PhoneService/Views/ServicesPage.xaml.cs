using GalaSoft.MvvmLight.Messaging;
using PhoneService.ViewModels;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PhoneService.Views
{
    /// <summary>
    /// Логика взаимодействия для ServicesPage.xaml
    /// </summary>
    public partial class ServicesPage : UserControl
    {
        private string _searchText = "";
        public ServicesPage(ServicesVM servicesVM)
        {
            InitializeComponent();
            DataContext = servicesVM;
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is ServicesVM servicesVM)
            {
                var searchText = (sender as TextBox)?.Text;
                if (searchText != null)
                {
                    servicesVM.SearchName = searchText;
                    servicesVM.Search.Execute(null);
                }
            }
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox textBox) return;

            string currentText = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength);

            string newText = currentText.Insert(textBox.SelectionStart, textBox.SelectedText);

            if (string.IsNullOrEmpty(newText))
            {
                textBox.Text = "0";
                return;
            }

            if (newText.Length > 1 && newText[0] == '0' && char.IsDigit(newText[1]))
            {
                textBox.Text = newText.Substring(1);
                return;
            }

            if (!Regex.IsMatch(newText, @"^(0|[1-9][0-9]*)$"))
            {
                textBox.Text = Regex.Replace(newText, @"[^0-9]", "");
                return;
            }

            try
            {
                Convert.ToInt32(newText);
            }
            catch
            {

                textBox.Text = int.MaxValue.ToString();
            }

            textBox.SelectionStart = textBox.Text.Length;
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            Messenger.Default.Send(false);
            if (DataContext is ServicesVM servicesVM)
            {
                servicesVM.IsEditing = true;
            }

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ServicesVM servicesVM)
            {
                servicesVM.Search.Execute(_searchText);
            }
        }
    }
}