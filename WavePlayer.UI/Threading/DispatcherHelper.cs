using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WavePlayer.UI.Threading
{
    public static class DispatcherHelper
    {
        public static Dispatcher UIDispatcher
        {
            get;
            private set;
        }

        public static void Initialize()
        {
            UIDispatcher = Dispatcher.CurrentDispatcher;
        }

        public static void InvokeOnUI(Action action)
        {
            CheckDispatcher();

            if (UIDispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                UIDispatcher.InvokeAsync(action);
            }
        }

        private static void CheckDispatcher()
        {
            if (UIDispatcher == null)
            {
                throw new InvalidOperationException("Dispatcher helper was not initialized");
            }
        }
    }
}
