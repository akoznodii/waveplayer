using System;
using System.Runtime.InteropServices;

namespace WavePlayer.Fmod.Native
{
    internal abstract class Handle : SafeHandle
    {
        protected Handle() : this(IntPtr.Zero)
        {
        }

        protected Handle(IntPtr handle) : this(handle, true)
        {
        }

        protected Handle(IntPtr handle, bool ownsHandle) : base(IntPtr.Zero, ownsHandle)
        {
            SetHandle(handle);
        }

        public override bool IsInvalid
        {
            get
            {
                return (handle == IntPtr.Zero || (int)handle == -1);
            }
        }
    }
}
