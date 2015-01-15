using System;
using System.Threading.Tasks;

namespace WavePlayer.UI.Commands
{
    public class AsyncCommand<T> : RelayCommand<T>
    {
        public AsyncCommand(Action<T> execute)
            : base(execute)
        {
        }

        public AsyncCommand(Action<T> execute, Func<T, bool> canExecute)
            : base(execute, canExecute)
        {
        }

        public AsyncCommand(Action<T> execute, Func<T, bool> canExecute, Func<object, T> converter)
            : base(execute, canExecute, converter)
        {
        }

        private bool IsRunningNow { get; set; }

        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter) && !IsRunningNow;
        }

        public override void Execute(object parameter)
        {
            Task.Factory.StartNew(() =>
            {
                IsRunningNow = true;

                try
                {
                    base.Execute(parameter);
                }
                finally
                {
                    IsRunningNow = false;
                }
            });
        }
    }
}
