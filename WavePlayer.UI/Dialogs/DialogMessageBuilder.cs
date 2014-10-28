using System;
using WavePlayer.UI.Properties;

namespace WavePlayer.UI.Dialogs
{
    public static class DialogMessageBuilder
    {
        public static DialogMessage CreateMessage(Exception exception)
        {
            return new DialogMessage()
            {
                Title = Resources.Error,
                Message = exception.Message,
                AffirmativeActionText = Resources.Ok
            };
        }

        public static DialogMessage CreateMessage(Exception exception, Action retryAction)
        {
            var message = CreateMessage(exception);

            message.AffirmativeActionText = Resources.Retry;
            message.AffirmativeAction = retryAction;
            message.NegativeActionText = Resources.Ok;
            
            return message;
        }
    }
}
