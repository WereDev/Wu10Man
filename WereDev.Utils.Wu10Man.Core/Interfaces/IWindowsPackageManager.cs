using System.Collections.Generic;
using WereDev.Utils.Wu10Man.Core.Models;

namespace WereDev.Utils.Wu10Man.Core.Interfaces
{
    public interface IWindowsPackageManager
    {
        Declutter GetDeclutterConfig();

        PackageInfo[] ListInstalledPackages();

        AppInfoExtended[] MergePackageInfo(IEnumerable<AppInfo> apps, IEnumerable<PackageInfo> packages);

        void RemovePackage(string packageFullName);
    }
}
