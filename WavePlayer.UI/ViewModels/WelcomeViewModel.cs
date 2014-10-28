using WavePlayer.UI.Properties;

namespace WavePlayer.UI.ViewModels
{
    public sealed class WelcomeViewModel : HostViewModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string ProductCopyrightString
        {
            get { return Resources.Copyright; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string ProductNameString
        {
            get { return Resources.ProductNameTitle; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Localizable resource string")]
        public string ProductSloganString
        {
            get { return Resources.ProductSlogan; }
        }

        public override void UpdateLocalization()
        {
            base.UpdateLocalization();

            RaisePropertyChanged("ProductCopyrightString");
            RaisePropertyChanged("ProductNameString");
            RaisePropertyChanged("ProductSloganString");
        }
    }
}
