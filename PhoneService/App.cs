using System.Windows;

namespace PhoneService
{
    internal class App : Application
    {
        private readonly MainWindow _mainWindow;
        public App(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            App.Current.MainWindow = _mainWindow;
            new ExceptionHandler().Register();
            _mainWindow.Show();
            base.OnStartup(e);
        }
    }
}
