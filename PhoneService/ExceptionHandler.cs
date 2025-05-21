using GalaSoft.MvvmLight.Messaging;
using System.Windows;

public class ExceptionHandler
{
    public void Register()
    {
        Application.Current.DispatcherUnhandledException += DispatcherUnhandledExceptionHandler;
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
    }

    private void DispatcherUnhandledExceptionHandler(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        HandleException(e.Exception);
        e.Handled = true;
    }

    private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
    {
        HandleException(e.ExceptionObject as Exception);
    }

    private void HandleException(Exception? ex)
    {
        Messenger.Default.Send(ex.Message);
    }

}
