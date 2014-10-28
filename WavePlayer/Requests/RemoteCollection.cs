using System.Collections.Generic;

namespace WavePlayer.Requests
{
    public sealed class RemoteCollection<TItem> : List<TItem>
    {
        public RequestBase Request { get; set; }

        public bool CanLoad
        {
            get
            {
                return !Loaded || (Loaded && Count < TotalCount);
            }
        }

        public bool Loaded { get; set; }

        public int TotalCount { get; set; }
    }
}
