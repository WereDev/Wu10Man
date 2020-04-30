using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    PackageFullName = package.Id.FullName,
                    PackageName = package.Id.Name,
                };
                packageInfoList.Add(packageInfo);
            }

            return packageInfoList.ToArray();
        }

        public string GetLogoUrl(PackageInfo packageInfo)
        {
            return string.Empty;
        }

        private string GetLogoUrl(string packagePath)
        {
            var manifest = GetAppManifest(packagePath);
            var manifestIconPath = Path.Combine(packagePath, manifest.Properties.Logo);

            var iconDirectory = Path.GetDirectoryName(manifestIconPath);
            var iconFileName = Path.GetFileNameWithoutExtension(manifestIconPath) + ".scale-100." + Path.GetExtension(manifestIconPath);
            var iconPath = Path.Combine(iconDirectory, iconFileName);
            return iconPath;
        }

        private Package GetAppManifest(string packagePath)
        {
            

            var manifestPath = Path.Combine(packagePath, "AppxManifest.xml");
            var xDoc = new XmlDocument();

            xDoc.Load(manifestPath);

            var xNodeReader = new XmlNodeReader(xDoc.DocumentElement);

            var xmlSerializer = new XmlSerializer(typeof(Package));

            var employeeData = xmlSerializer.Deserialize(xNodeReader);

            Package deserializedEmployee = (Package)employeeData;

            return deserializedEmployee;
        }

        public void RemovePackage(string packageFullName)
        {
            var task = _packageManager.RemovePackageAsync(packageFullName, RemovalOptions.RemoveForAllUsers);
            var results = task.GetResults();
        }
    }
}
