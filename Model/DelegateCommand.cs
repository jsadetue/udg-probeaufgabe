using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UdgChallenge.Model
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _executeAction;
        private readonly Func<object, bool> _canExecuteAction;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> executeAction)
        {
            _executeAction = executeAction;
        }

        public DelegateCommand(Func<object, bool> canExecuteAction, Action<object> executeAction)
        {
            _canExecuteAction = canExecuteAction;
            _executeAction = executeAction;
        }

        public bool CanExecute(object parameter) => _canExecuteAction?.Invoke(parameter) ?? true;
        public void Execute(object parameter) => _executeAction(parameter);

        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
