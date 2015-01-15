using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;

namespace WavePlayer.UI.Controls.Dialogs
{
    public partial class SimpleMessageDialog : CustomDialog
    {
        internal SimpleMessageDialog()
        {
            InitializeComponent();
        }

        internal Task<MessageDialogResult> WaitForButtonPressAsync()
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Focus();

                    //kind of acts like a selective 'IsDefault' mechanism.
                    if (ButtonStyle == MessageDialogStyle.Affirmative)
                        PART_AffirmativeButton.Focus();
                    else if (ButtonStyle == MessageDialogStyle.AffirmativeAndNegative)
                        PART_NegativeButton.Focus();
                }));

            TaskCompletionSource<MessageDialogResult> tcs = new TaskCompletionSource<MessageDialogResult>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            Action cleanUpHandlers = () =>
            {
                PART_NegativeButton.Click -= negativeHandler;
                PART_AffirmativeButton.Click -= affirmativeHandler;

                PART_NegativeButton.KeyDown -= negativeKeyHandler;
                PART_AffirmativeButton.KeyDown -= affirmativeKeyHandler;
            };
            
            negativeKeyHandler = new KeyEventHandler((sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.Negative);
                    }
                });

            affirmativeKeyHandler = new KeyEventHandler((sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.Affirmative);
                    }
                });
            
            negativeHandler = new RoutedEventHandler((sender, e) =>
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Negative);

                    e.Handled = true;
                });

            affirmativeHandler = new RoutedEventHandler((sender, e) =>
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Affirmative);

                    e.Handled = true;
                });

            PART_NegativeButton.KeyDown += negativeKeyHandler;
            PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;

            PART_NegativeButton.Click += negativeHandler;
            PART_AffirmativeButton.Click += affirmativeHandler;

            return tcs.Task;
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(SimpleMessageDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register("AffirmativeButtonText", typeof(string), typeof(SimpleMessageDialog), new PropertyMetadata("OK"));
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(SimpleMessageDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register("ButtonStyle", typeof(MessageDialogStyle), typeof(SimpleMessageDialog), new PropertyMetadata(MessageDialogStyle.Affirmative, new PropertyChangedCallback((s, e) =>
            {
                var md = (SimpleMessageDialog)s;

                SetButtonState(md);
            })));

        private static void SetButtonState(SimpleMessageDialog md)
        {
            if (md.PART_AffirmativeButton == null) return;

            switch (md.ButtonStyle)
            {
                case MessageDialogStyle.Affirmative:
                    {
                        md.PART_AffirmativeButton.Visibility = Visibility.Visible;
                        md.PART_NegativeButton.Visibility = Visibility.Collapsed;
                    }
                    break;
                case MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary:
                case MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary:
                case MessageDialogStyle.AffirmativeAndNegative:
                    {
                        md.PART_AffirmativeButton.Visibility = Visibility.Visible;
                        md.PART_NegativeButton.Visibility = Visibility.Visible;
                    }
                    break;
            }
        }

        public MessageDialogStyle ButtonStyle
        {
            get { return (MessageDialogStyle)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public string AffirmativeButtonText
        {
            get { return (string)GetValue(AffirmativeButtonTextProperty); }
            set { SetValue(AffirmativeButtonTextProperty, value); }
        }

        public string NegativeButtonText
        {
            get { return (string)GetValue(NegativeButtonTextProperty); }
            set { SetValue(NegativeButtonTextProperty, value); }
        }

        private void DialogLoaded(object sender, RoutedEventArgs e)
        {
            SetButtonState(this);
        }
    }
}
