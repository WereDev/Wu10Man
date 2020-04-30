using WereDev.Utils.Wu10Man.Core.Models;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class PackageInfo : ModelBase
    {
        private string _appName = string.Empty;
        private string _packageName = string.Empty;
        private bool _isInstalled = false;
        private string _iconPath = string.Empty;
        private bool _checkedForRemoval = false;

        public PackageInfo(AppInfoExtended appInfo)
        {
            _appName = appInfo.AppName;
            _iconPath = appInfo.IconPath;
            _isInstalled = appInfo.IsInstalled;
            _packageName = appInfo.PackageFullName;
        }

        public string AppName
        {
            get
            {
                return _appName;
            }

            set
            {
                _appName = value;
                OnPropertyChanged(nameof(AppName));
            }
        }

        public string PackageName
        {
            get
            {
                return _packageName;
            }

            set
            {
                _packageName = value;
                OnPropertyChanged(nameof(PackageName));
            }
        }

        public bool IsInstalled
        {
            get
            {
                return _isInstalled;
            }

            set
            {
                _isInstalled = value;
                OnPropertyChanged(nameof(IsInstalled));
            }
        }

        public string IconPath
        {
            get
            {
                return _iconPath;
            }

            set
            {
                _iconPath = value;
                OnPropertyChanged(nameof(IconPath));
            }
        }

        public bool CheckedForRemoval
        {
            get
            {
                return _checkedForRemoval;
            }

            set
            {
                _checkedForRemoval = value;
                OnPropertyChanged(nameof(CheckedForRemoval));
            }
        }
    }
}
