using GalaSoft.MvvmLight.Messaging;
using PhoneService.ViewModels;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PhoneService.Views
{
    /// <summary>
    /// Логика взаимодействия для StoragePage.xaml
    /// </summary>
    public partial class StoragePage : UserControl
    {
        public StoragePage(StorageVM storageVM)
        {
            InitializeComponent();
            DataContext = storageVM;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is StorageVM storageVM)
            {
                var searchText = (sender as TextBox)?.Text;
                if (searchText != null)
                {
                    storageVM.Search.Execute(searchText);
                }
            }
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox textBox) return;

            string currentText = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength);

            string newText = currentText.Insert(textBox.SelectionStart, textBox.SelectedText);

            if (!Regex.IsMatch(newText, @"^(0|[1-9][0-9]*)$"))
            {
                textBox.Text = Regex.Replace(newText, @"[^0-9]", "");
                return;
            }

            try
            {
                if (string.IsNullOrEmpty(newText))
                {
                    textBox.Text = "0";
                    return;
                }
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
            if (DataContext is StorageVM storageVM)
            {
                storageVM.IsEditing = true;
            }
        }
    }
}
