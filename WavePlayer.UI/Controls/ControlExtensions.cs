using System;
using System.Windows.Controls;

namespace WavePlayer.UI.Controls
{
    internal static class ControlExtensions
    {
        public static void InvokeIfRequired(this Control control, Action action)
        {
            if (control.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                control.Dispatcher.InvokeAsync(action);
            }
        }
    }
}
