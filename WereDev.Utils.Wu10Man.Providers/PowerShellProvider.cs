using System.Collections.Generic;
using System.Management.Automation;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;
using WereDev.Utils.Wu10Man.Core.Models;

namespace WereDev.Utils.Wu10Man.Providers
{
    public class PowerShellProvider : IWindowsPackageProvider
    {
        private const string CommandGetPackages = "Get-AppxPackage";
        private const string CommandRemovePackage = "Get-AppxPackage *{0}* | Remove-AppxPackage";
        private const string CommandRemoveProvisionedPackage = "Get-AppxProvisionedPackage -Online | where Displayname -EQ *{0}*| Remove-AppxProvisionedPackage -Online";

        public PackageInfo[] ListInstalledPackages()
        {
            var packages = new List<PackageInfo>();
            using (var ps = PowerShell.Create())
            {
                ps.AddCommand(CommandGetPackages);
                var results = ps.Invoke();

                foreach (var result in results)
                {
                    dynamic appx = result.BaseObject;
                    var package = new PackageInfo()
                    {
                        PackageName = appx.Name,
                    };
                    packages.Add(package);
                }
            }

            return packages.ToArray();
        }

        public void RemovePackage(string packageName)
        {
            using (var ps = PowerShell.Create())
            {
                ps.AddScript(string.Format(CommandRemovePackage, packageName));
                ps.AddScript(string.Format(CommandRemoveProvisionedPackage, packageName));
                var results = ps.Invoke();
            }
        }
    }
}
