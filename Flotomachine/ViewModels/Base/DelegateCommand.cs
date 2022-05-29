using System;
using System.Windows.Input;

namespace Flotomachine.ViewModels
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _action;
        public DelegateCommand(Action<object> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}