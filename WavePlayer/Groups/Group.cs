using System;
using VK.Groups;
using WavePlayer.Common;

namespace WavePlayer.Groups
{
    public class Group : ModelBase
    {
        private long _id;
        private string _name;
        private Uri _photo;

        public long Id
        {
            get
            {
                return _id;
            }

            set
            {
                SetField(ref _id, value);
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetField(ref _name, value);
            }
        }

        public GroupType GroupType { get; set; }

        public Uri Photo
        {
            get
            {
                return _photo;
            }

            set
            {
                SetField(ref _photo, value);
            }
        }
    }
}
