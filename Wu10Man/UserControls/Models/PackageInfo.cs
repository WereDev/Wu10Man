using System;
using System.Windows;
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
            if (appInfo == null)
                throw new ArgumentNullException(nameof(appInfo));

            _appName = appInfo.AppName;
            _iconPath = appInfo.IconPath;
            _isInstalled = appInfo.IsInstalled;
            _packageName = appInfo.PackageName;
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
                TriggerPropertyChanged(nameof(AppName));
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
                TriggerPropertyChanged(nameof(PackageName));
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
                TriggerPropertyChanged(nameof(IsInstalled), nameof(GetVisibility));
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
                TriggerPropertyChanged(nameof(IconPath));
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
                TriggerPropertyChanged(nameof(CheckedForRemoval));
            }
        }

        public Visibility GetVisibility => IsInstalled ? Visibility.Visible : Visibility.Collapsed;

        public override string ToString()
        {
            return _appName;
        }
    }
}
