using System;
using System.Linq;
using WereDev.Utils.Wu10Man.Interfaces;

namespace WereDev.Utils.Wu10Man.Utilites
{
    internal class WindowsServiceManager : IWindowsServiceManager
    {
        private readonly string _wu10FilePrefix = "Wu10Man_";

        private readonly IWindowsServiceProviderFactory _providerFactory;
        private readonly IFilesHelper _filesHelper;

        public WindowsServiceManager(IWindowsServiceProviderFactory providerFactory, IFilesHelper filesHelper)
        {
            _providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
            _filesHelper = filesHelper ?? throw new ArgumentNullException(nameof(filesHelper));
        }

        public string[] ListAllServices()
        {
            return new string[]
            {
                Constants.SERVICE_WINDOWS_UPDATE,
                Constants.SERVICE_UPDATE_MEDIC,
                Constants.SERVICE_SHOULD_NOT_EXIST,
                Constants.SERVICE_MODULES_INSTALLER
            };
        }

        public bool ServiceExists(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            using (var service = _providerFactory.GetWindowsServiceProvider(serviceName))
            {
                return service.ServiceExists();
            }
        }

        public string GetServiceDisplayName(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            using (var service = _providerFactory.GetWindowsServiceProvider(serviceName))
            {
                return service.DisplayName;
            }
        }

        public string GetServiceDllPath(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            using (var service = _providerFactory.GetWindowsServiceProvider(serviceName))
            {
                var dll = service.GetServiceDLL();
                return dll;
            }
        }

        public bool AreAllServicesEnabled()
        {
            return ListAllServices().All(x => IsServiceEnabled(x));
        }

        public bool AreAllServicesDisabled()
        {
            return ListAllServices().All(x => !IsServiceEnabled(x));
        }

        public bool IsServiceEnabled(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            var serviceDllPath = GetServiceDllPath(serviceName);

            using (var service = _providerFactory.GetWindowsServiceProvider(serviceName))
            {
                return service.IsServiceEnabled()
                       && (string.IsNullOrEmpty(serviceDllPath) || _filesHelper.Exists(serviceDllPath));
            }
        }

        public bool EnableService(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            RemoveWu10FromFileName(serviceName);
            var enabledRealtime = false;
            using (var service = _providerFactory.GetWindowsServiceProvider(serviceName))
            {
                if (!service.IsServiceRunAsLocalSystem())
                    service.SetAccountAsLocalSystem();
                enabledRealtime = service.EnableService();
            }
            return enabledRealtime;
        }

        public void DisableService(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            using (var service = _providerFactory.GetWindowsServiceProvider(serviceName))
            {
                service.DisableService();
            }
            AddWu10ToFileName(serviceName);
        }

        public void AddWu10ToFileName(string serviceName)
        {
            var dllPath = GetServiceDllPath(serviceName);
            if (string.IsNullOrEmpty(dllPath)) return;

            _filesHelper.GiveOwnershipToAdministrators(dllPath);
            var wu10Path = GetPathWithWu10Prefix(dllPath);
            _filesHelper.RenameFile(dllPath, wu10Path);
        }

        public void RemoveWu10FromFileName(string serviceName)
        {
            var dllPath = GetServiceDllPath(serviceName);
            if (string.IsNullOrEmpty(dllPath)) return;

            var wu10Path = GetPathWithWu10Prefix(dllPath);

            // If the file has returned, then we need to assume that some other process has recreated
            // it.  It's safer to assume the new file is correct and delete the "old" file.
            if (_filesHelper.Exists(dllPath))
            {
                _filesHelper.Delete(wu10Path);
            }
            else
            {
                _filesHelper.GiveOwnershipToAdministrators(wu10Path);
                _filesHelper.RenameFile(wu10Path, dllPath);
                _filesHelper.GiveOwnershipToTrustedInstaller(dllPath);
            }
        }

        private string GetPathWithWu10Prefix(string path)
        {
            var folder = _filesHelper.GetDirectoryName(path);
            var fileName = _filesHelper.GetFileName(path);
            if (fileName.StartsWith(_wu10FilePrefix, StringComparison.CurrentCultureIgnoreCase))
                return path;

            fileName = _wu10FilePrefix + fileName;
            var newPath = _filesHelper.Combine(folder, fileName);
            return newPath;
        }
    }
}
