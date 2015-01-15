using System;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using WavePlayer.UI.Controls;
using WavePlayer.UI.Controls.Dialogs;
using WavePlayer.UI.Dialogs;

namespace WavePlayer.UI.Windows
{
    public partial class HostWindow : MetroWindow, IDialogService
    {
        private Task<ProgressDialogController> _task;

        public HostWindow()
        {
            InitializeComponent();
        }

        public void NotifyError(Exception exception)
        {
            var message = DialogMessageBuilder.CreateMessage(exception);

            NotifyMessage(message);
        }

        public void NotifyError(Exception exception, Action retryAction)
        {
            var message = DialogMessageBuilder.CreateMessage(exception, retryAction);

            NotifyMessage(message);
        }

        public void StartProgress(string message = null)
        {
            this.InvokeIfRequired(() => StartProgressAsync(message));
        }

        public void StopProgress()
        {
            StopProgressAsync();
        }

        public void NotifyMessage(DialogMessage message)
        {
            var stopTask = StopProgressAsync();

            if (stopTask != null)
            {
                stopTask.Wait();
            }

            this.InvokeIfRequired(() => ShowDialog(message));
        }

        public void ShowCaptcha(CaptchaRequest request)
        {
            if (Dispatcher.CheckAccess())
            {
                throw new InvalidOperationException("Captcha dialog cannot be shown from UI thread");
            }

            Task task = null;

            Dispatcher.Invoke(() => task = ShowCaptchaAsync(request));

            task.Wait();
        }

        private async Task ShowCaptchaAsync(CaptchaRequest request)
        {
            var captchaDialog = new CaptchaDialog
            {
                Title = Properties.Resources.Captcha,
                Message = Properties.Resources.PleaseEnterTheCodeShownBelow,
                AffirmativeButtonText = Properties.Resources.Ok,
                NegativeButtonText = Properties.Resources.Close,
                Source = request.Source
            };

            await this.ShowMetroDialogAsync(captchaDialog);

            request.Text = await captchaDialog.WaitForButtonPressAsync();

            await this.HideMetroDialogAsync(captchaDialog);
        }

        private void StartProgressAsync(string message)
        {
            _task = this.ShowProgressAsync(Properties.Resources.PleaseWait, message);
        }

        private Task StopProgressAsync()
        {
            var controller = WaitForStartProgressTask();

            return controller == null ? null : controller.CloseAsync();
        }

        private async void ShowDialog(DialogMessage message)
        {
            var dialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = message.AffirmativeActionText,
                NegativeButtonText = message.NegativeActionText
            };

            var buttonStyle = string.IsNullOrEmpty(message.NegativeActionText)
                ? MessageDialogStyle.Affirmative
                : MessageDialogStyle.AffirmativeAndNegative;

            var task = this.ShowMessageAsync(message.Title, message.Message, buttonStyle, dialogSettings);

            await task;

            Action action = null;

            switch (task.Result)
            {
                case MessageDialogResult.Affirmative:
                    action = message.AffirmativeAction;
                    break;
                case MessageDialogResult.Negative:
                    action = message.NegativeAction;
                    break;
            }

            if (action != null)
            {
                await Dispatcher.InvokeAsync(action);
            }
        }

        private ProgressDialogController WaitForStartProgressTask()
        {
            if (_task == null)
            {
                return null;
            }

            var controller = _task.Result;

            _task = null;

            return controller;
        }
    }
}
