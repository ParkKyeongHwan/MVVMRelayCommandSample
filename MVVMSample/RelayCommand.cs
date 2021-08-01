using System;
using System.Windows.Input;

namespace MVVMSample
{
    public class RelayCommand : ICommand
    {
        private Func<object, bool> canExecute;
        private Action<object> executeAction;

        public RelayCommand(Action<object> executeAction) : this(executeAction, null)
        {

        }

        public RelayCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            this.executeAction = executeAction ??
                throw new ArgumentNullException("Execute Action was null for ICommanding");
            this.canExecute = canExecute;
        }
        
        public bool CanExecute(object parameter)
        {
            if (parameter?.ToString().Length is 0)
            {
                return false;
            }

            bool result = this.canExecute is null ? true : this.canExecute.Invoke(parameter);
            return result;
        }

        public void Execute(object parameter)
        {
            this.executeAction.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }


    }
}
