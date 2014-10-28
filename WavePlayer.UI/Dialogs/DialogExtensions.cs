using System;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using WavePlayer.UI.Common;

namespace WavePlayer.UI.Dialogs
{
    internal static class DialogExtensions
    {
        public static void ShowDialog(this MetroWindow metroWindow, DialogMessage message)
        {
            metroWindow.InvokeIfRequired(() => ShowDialogInternal(metroWindow, message));
        }

        private static void ShowDialogInternal(MetroWindow metroWindow, DialogMessage message)
        {
            var dialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = message.AffirmativeButtonText,
                NegativeButtonText = message.NegativeButtonText
            };

            var buttonStyle = string.IsNullOrEmpty(message.NegativeButtonText)
                ? MessageDialogStyle.Affirmative
                : MessageDialogStyle.AffirmativeAndNegative;

            metroWindow.ShowMessageAsync(message.Title, message.Message, buttonStyle, dialogSettings)
                .ContinueWith(task =>
                {
                    Action action = null;

                    switch (task.Result)
                    {
                        case MessageDialogResult.Affirmative:
                            action = message.AffirmativeCallback;
                            break;
                        case MessageDialogResult.Negative:
                            action = message.NegativeCallback;
                            break;
                    }

                    if (action != null)
                    {
                        metroWindow.Dispatcher.InvokeAsync(action);
                    }
                });
        }
    }
}
