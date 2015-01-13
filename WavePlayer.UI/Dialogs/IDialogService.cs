using System;

namespace WavePlayer.UI.Dialogs
{
    public interface IDialogService
    {
        void NotifyError(Exception exception);

        void NotifyError(Exception exception, Action retryAction);

        void NotifyMessage(DialogMessage message);

        void StartProgress(string message = null);

        void StopProgress();
    }
}
