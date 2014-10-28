using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavePlayer.UI.Dialogs
{
    public class DialogMessage
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public string AffirmativeActionText { get; set; }

        public Action AffirmativeAction { get; set; }

        public string NegativeActionText { get; set; }

        public Action NegativeAction { get; set; }
    }
}
