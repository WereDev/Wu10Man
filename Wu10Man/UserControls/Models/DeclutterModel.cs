using System.Collections.Generic;
using System.Linq;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class DeclutterModel : ModelBase
    {
        private PackageInfo[] _microsoftPackages = new PackageInfo[0];
        private PackageInfo[] _thirdPartyPackages = new PackageInfo[0];
        private PackageSource _activeSource = PackageSource.Microsoft;

        public enum PackageSource
        {
            Microsoft,
            ThirdParty,
        }

        public PackageInfo[] Packages { get; private set; }

        public bool AllPackagesSelected => !(Packages?.Any(x => !x.CheckedForRemoval) ?? false);

        public string SelectButtonText => AllPackagesSelected ? "Deselect All Apps" : "Select All Apps";

        public PackageSource ActiveSource
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

        public void SetPackages(PackageSource packageSource, IEnumerable<PackageInfo> packages)
        {
            packages = packages ?? System.Array.Empty<PackageInfo>();

            switch (packageSource)
            {
                case PackageSource.Microsoft:
                    _microsoftPackages = packages.ToArray();
                    break;
                case PackageSource.ThirdParty:
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
            Packages = _activeSource == PackageSource.Microsoft
                           ? _microsoftPackages
                           : _thirdPartyPackages;

            Packages = Packages.Where(x => x.IsInstalled).OrderBy(x => x.AppName).ToArray();
        }
    }
}
