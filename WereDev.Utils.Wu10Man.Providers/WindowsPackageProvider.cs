using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;
using WereDev.Utils.Wu10Man.Core.Models;
using WereDev.Utils.Wu10Man.Providers.Models;
using Windows.Management.Deployment;
using PackageManager = Windows.Management.Deployment.PackageManager;

namespace WereDev.Utils.Wu10Man.Providers
{
    public class WindowsPackageProvider : IWindowsPackageProvider
    {
        private readonly PackageManager _packageManager;

        public WindowsPackageProvider()
        {
            _packageManager = new PackageManager();
        }

        public PackageInfo[] ListInstalledPackages()
        {
            var packages = _packageManager.FindPackages().ToArray();

            var packageInfoList = new List<PackageInfo>();

            foreach(var package in packages)
            {
                var packageInfo = new PackageInfo
                {
                    InstallLocation = package.InstalledLocation.Path,
                    PackageName = package.Id.Name,
                };
                packageInfoList.Add(packageInfo);
            }

            return packageInfoList.ToArray();
        }

        public void RemovePackage(string packageName)
        {
            var package = _packageManager.FindPackage(packageName);
            var installerInfo = package.GetAppInstallerInfo();

            //var task = _packageManager.RemovePackageAsync(packageFullName, RemovalOptions.RemoveForAllUsers);
            //var completed = new AutoResetEvent(false);
            //task.Completed = (waitResult, status) => { completed.Set(); } ;
            //completed.WaitOne();
            //var result = task.GetResults();
        }
    }
}
