using System;
using System.Windows.Input;

namespace PastPaperHelper
{
    public class DelegateCommand : ICommand
    {
        Action<object> _execute;
        Func<object, bool> _canExecute;

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null) return true;
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            if (_execute != null && CanExecute(parameter)) _execute(parameter);
        }
    }
}
