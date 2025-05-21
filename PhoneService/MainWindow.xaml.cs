using Microsoft.EntityFrameworkCore.Infrastructure;
using PhoneService.Models;
using PhoneService.ViewModels;
using System.Windows;

namespace PhoneService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ViewsVM _viewsVM;
        private readonly Mediator _mediator;
        private bool _isLight = true;
        public static bool IsRu { get; private set; } = true;
        public MainWindow(
            ViewsVM viewsVM,
            Mediator mediator
        )
        {
            InitializeComponent();
            DataContext = viewsVM;
            _viewsVM = viewsVM;
            _mediator = mediator;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Resources.MergedDictionaries.Clear();
            if (_isLight)
            {
                Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/Resources/DarkTheme.xaml", UriKind.Absolute)
                });
            }
            else
            {
                Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/Resources/LightTheme.xaml", UriKind.Absolute)
                });
            }
            if (IsRu)
            {
                Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/Resources/LanguageRu.xaml", UriKind.Absolute)
                });
            }
            else
            {
                Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/Resources/LanguageEn.xaml", UriKind.Absolute)
                });
            }
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Resources/Styles.xaml", UriKind.Absolute)
            });
            _isLight = !_isLight;
            _viewsVM.ChangeTheme.Execute(null);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Resources.MergedDictionaries.Clear();
            if (_isLight)
            {
                Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/Resources/LightTheme.xaml", UriKind.Absolute)
                });
            }
            else
            {
                Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/Resources/DarkTheme.xaml", UriKind.Absolute)
                });
            }
            if (IsRu)
            {
                Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/Resources/LanguageEn.xaml", UriKind.Absolute)
                });
            }
            else
            {
                Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/Resources/LanguageRu.xaml", UriKind.Absolute)
                });
            }
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Resources/Styles.xaml", UriKind.Absolute)
            });
            IsRu = !IsRu;
            BLL.Services.ModelValidator.IsRu = IsRu;
            _mediator.UpdateServices();
            _mediator.UpdateUser();
        }
    }
}