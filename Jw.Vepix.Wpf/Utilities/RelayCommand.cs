using System;
using System.Windows.Input;

namespace JW.Vepix.Wpf.Utilities
{
    public class RelayCommand<T> : ICommand
    {
        private Action _targetExecuteMethod;
        private Action<T> _tTargetExecuteMethod;
        private Func<bool> _targetCanExecuteMethod;

        public RelayCommand(Action executeMethod)
        {
            _targetExecuteMethod = executeMethod;
        }

        public RelayCommand(Action<T> executeMethod)
        {
            _tTargetExecuteMethod = executeMethod;
        }

        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            _targetExecuteMethod = executeMethod;
            _targetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        #region ICommand members
        public bool CanExecute(object parameter)
        {
            if (_targetCanExecuteMethod != null)
            {
                return _targetCanExecuteMethod();
            }
            if (_targetExecuteMethod != null || _tTargetExecuteMethod != null)
            {
                return true;
            }
            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public void Execute(object parameter)
        {
            if (_targetExecuteMethod != null)
            {
                _targetExecuteMethod();
            }
            else
            {
                _tTargetExecuteMethod((T)parameter);
            }
        }
        #endregion
    }
}
