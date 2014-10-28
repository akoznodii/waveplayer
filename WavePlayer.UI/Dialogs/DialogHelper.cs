using System;
using WavePlayer.UI.Properties;

namespace WavePlayer.UI.Dialogs
{
    public static class DialogHelper
    {
        public static DialogMessage BuildMessage(Exception exception)
        {
            var message = exception.Message;

            return new DialogMessage()
            {
                Title = Resources.Error,
                AffirmativeButtonText = Resources.Ok,
                Message = message
            };
        }
    }
}
