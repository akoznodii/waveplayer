using WavePlayer.Common;

namespace WavePlayer.Audios
{
    public class Genre : ModelBase
    {
        private string _name;

        public int Id { get; set; }

        public string Name { get { return _name; } set { SetField(ref _name, value); } }
    }
}
