using System;

namespace WavePlayer.Fmod.Native
{
    internal class Dsp : Handle
    {
        internal Dsp(IntPtr dspHandle)
        {
            SetHandle(dspHandle);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May be used later")]
        public bool Bypass
        {
            get
            {
                var result = false;
                ErrorHandler.ThrowIfError(NativeMethods.GetBypass(DangerousGetHandle(), out result));
                return result;
            }

            set
            {
                ErrorHandler.ThrowIfError(NativeMethods.SetBypass(DangerousGetHandle(), value));
            }
        }

        public float GetParameter(int index)
        {
            var value = 0f;

            ErrorHandler.ThrowIfError(NativeMethods.GetParameter(DangerousGetHandle(), index, out value, null, 0));

            return value;
        }

        public void SetParameter(int index, float value)
        {
            ErrorHandler.ThrowIfError(NativeMethods.SetParameter(DangerousGetHandle(), index, value));
        }

        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
            {
                return true;
            }

            NativeMethods.ReleaseDsp(handle);
            SetHandleAsInvalid();
            return true;
        }
    }
}
