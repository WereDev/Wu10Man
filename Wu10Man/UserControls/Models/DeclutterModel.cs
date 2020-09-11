using System.Collections.Generic;
using System.Linq;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    internal class DeclutterModel : ModelBase
    {
        private PackageInfo[] _microsoftPackages = new PackageInfo[0];
        private PackageInfo[] _thirdPartyPackages = new PackageInfo[0];
        private PackageSources _activeSource = PackageSources.Microsoft;

        public enum PackageSources
        {
            Microsoft,
            ThirdParty,
        }

        public PackageInfo[] Packages { get; private set; }

        public bool AllPackagesSelected => !Packages.Any(x => !x.CheckedForRemoval);

        public string SelectButtonText => AllPackagesSelected ? "Deselect All Apps" : "Select All Apps";

        public PackageSources PackageSource
        {
            get
            {
                return _activeSource;
            }

            set
            {
                _activeSource = value;
                SetActivePackages();
                TriggerPropertyChanged(nameof(Packages), nameof(SelectButtonText));
            }
        }

        public void SetPackages(PackageSources packageSource, IEnumerable<PackageInfo> packages)
        {
            switch (packageSource)
            {
                case PackageSources.Microsoft:
                    _microsoftPackages = packages.ToArray();
                    break;
                case PackageSources.ThirdParty:
                    _thirdPartyPackages = packages.ToArray();
                    break;
            }

            foreach (var package in packages)
            {
                package.PropertyChanged += (sender, e) =>
                {
                    TriggerPropertyChanged(nameof(Packages), nameof(SelectButtonText));
                };
            }

            SetActivePackages();
            TriggerPropertyChanged(nameof(Packages), nameof(SelectButtonText));
        }

        private void SetActivePackages()
        {
            Packages = _activeSource == PackageSources.Microsoft
                           ? _microsoftPackages
                           : _thirdPartyPackages;

            Packages = Packages.OrderBy(x => x.AppName).ToArray();
        }
    }
}
