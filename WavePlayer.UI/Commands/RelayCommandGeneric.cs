using System;
using System.Windows.Input;

namespace WavePlayer.UI.Commands
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;
        private readonly Func<object, T> _converter;

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;

            if (_execute == null)
            {
                throw new ArgumentNullException("execute");
            }
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute, Func<object, T> converter)
            : this(execute, canExecute)
        {
            _converter = converter;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            var value = Convert(parameter);

            return _canExecute(value);
        }

        public void Execute(object parameter)
        {
            var value = Convert(parameter);

            _execute(value);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As design")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "As design")]
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private T Convert(object parameter)
        {
            var value = default(T);

            if (parameter is T)
            {
                value = (T)parameter;
            }
            else if (_converter != null)
            {
                value = _converter(parameter);
            }

            return value;
        }
    }
}
