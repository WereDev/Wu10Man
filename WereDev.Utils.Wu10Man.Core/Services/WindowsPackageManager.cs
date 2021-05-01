using System;
using System.Collections.Generic;
using System.Linq;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;
using WereDev.Utils.Wu10Man.Core.Models;

namespace WereDev.Utils.Wu10Man.Core.Services
{
    public class WindowsPackageManager : IWindowsPackageManager
    {
        private readonly IWindowsPackageProvider _packageProvider;
        private readonly DeclutterConfig _declutterConfig;

        public WindowsPackageManager(IWindowsPackageProvider packageProvider, DeclutterConfig declutterConfig)
        {
            _packageProvider = packageProvider ?? throw new ArgumentNullException(nameof(packageProvider));
            _declutterConfig = declutterConfig ?? new DeclutterConfig();
        }

        public DeclutterConfig GetDeclutterConfig()
        {
            return _declutterConfig;
        }

        public PackageInfo[] ListInstalledPackages()
        {
            return _packageProvider.ListInstalledPackages();
        }

        public AppInfoExtended[] MergePackageInfo(IEnumerable<AppInfo> apps, IEnumerable<PackageInfo> packages)
        {
            List<AppInfoExtended> appInfos = new List<AppInfoExtended>();
            foreach (var app in apps)
            {
                var appInfo = new AppInfoExtended(app);
                var package = packages.FirstOrDefault(x => x.PackageName == app.PackageName);
                if (package != null)
                {
                    appInfo.IsInstalled = true;
                }

                appInfos.Add(appInfo);
            }

            return appInfos.ToArray();
        }

        public void RemovePackage(string packageName)
        {
            _packageProvider.RemovePackage(packageName);
        }
    }
}
