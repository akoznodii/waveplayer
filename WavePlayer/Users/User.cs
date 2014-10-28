using System;
using WavePlayer.Common;

namespace WavePlayer.Users
{
    public class User : ModelBase
    {
        private long _id;
        private string _firstName;
        private string _lastName;
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

        public string FirstName
        {
            get
            {
                return _firstName;
            }

            set
            {
                SetField(ref _firstName, value);
            }
        }

        public string LastName
        {
            get
            {
                return _lastName;
            }

            set
            {
                SetField(ref _lastName, value);
            }
        }

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
