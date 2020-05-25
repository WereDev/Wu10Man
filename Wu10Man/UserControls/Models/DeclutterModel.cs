namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    internal class DeclutterModel : ModelBase
    {
        private PackageInfo[] _microsoftPackages = new PackageInfo[0];
        private PackageInfo[] _thirdPartyPackages = new PackageInfo[0];

        public DeclutterModel()
        {
            PackageSource = PackageSources.Microsoft;
        }

        public enum PackageSources
        {
            Microsoft,
            ThirdParty,
        }

        public PackageSources PackageSource { get; set; }

        public PackageInfo[] MicrosoftPackages
        {
            get
            {
                return _microsoftPackages;
            }

            set
            {
                _microsoftPackages = value ?? new PackageInfo[0];
                OnPropertyChanged(nameof(MicrosoftPackages));
            }
        }

        public PackageInfo[] ThirdPartyPackages
        {
            get
            {
                return _thirdPartyPackages;
            }

            set
            {
                _thirdPartyPackages = value ?? new PackageInfo[0];
                OnPropertyChanged(nameof(ThirdPartyPackages));
            }
        }
    }
}
