using System;
using System.Windows.Threading;

namespace WavePlayer.UI.Common
{
    public static class ControlExtensions
    {
        public static void InvokeIfRequired(this DispatcherObject dispatcherObject, Action action)
        {
            if (dispatcherObject.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcherObject.Dispatcher.InvokeAsync(action);
            }
        }

        public static T InvokeIfRequired<T>(this DispatcherObject dispatcherObject, Func<T> function)
        {
            if (dispatcherObject.CheckAccess())
            {
                return function();
            }
            else
            {
               return dispatcherObject.Dispatcher.InvokeAsync(function).Result;
            }
        }
    }
}
