using WavePlayer.UI.Properties;

namespace WavePlayer.UI.ViewModels
{
    public sealed class LoadInfoViewModel : ViewModelBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string LoadAccountString
        {
            get { return Resources.LoadingAccountInformation; }
        }

        public override void UpdateLocalization()
        {
            base.UpdateLocalization();

            RaisePropertyChanged("LoadAccountString");
        }
    }
}
