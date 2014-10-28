using WavePlayer.Common;
using WavePlayer.Localization;

namespace WavePlayer.UI.ViewModels
{
    public abstract class ViewModelBase : ModelBase, ILocalizable
    {
        public virtual void UpdateLocalization()
        {
        }
    }
}
