using WavePlayer.Common;

namespace WavePlayer.Audios
{
    public class Lyrics : ModelBase
    {
        private string _text;

        public long Id { get; set; }

        public string Artist { get; set; }

        public string Title { get; set; }

        public string Text { get { return _text; } set { SetField(ref _text, value); } }
    }
}
