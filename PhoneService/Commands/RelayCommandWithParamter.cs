using System.Windows.Input;

namespace PhoneService.Commands
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> execute;
        private readonly Func<bool>? canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<T?> execute, Func<bool>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return this.canExecute == null || this.canExecute();
        }

        public void Execute(object? parameter)
        {
            this.execute((T?)parameter);
        }
    }
}
