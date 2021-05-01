using System;
using System.Linq;
using WereDev.Utils.Wu10Man.Core.Enums;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;

namespace WereDev.Utils.Wu10Man.Core.Services
{
    public class WindowsServiceManager : IWindowsServiceManager
    {
        private readonly string _wu10FilePrefix = "Wu10Man_";
        private readonly string _serviceRegistryPath = @"SYSTEM\CurrentControlSet\Services\";
        private readonly IWindowsServiceProviderFactory _providerFactory;
        private readonly IRegistryEditor _registryEditor; // TODO: Not a fan of this because of possible circular reference
        private readonly IFileManager _fileManager;
        private readonly string[] _windowsServices;

        public WindowsServiceManager(IWindowsServiceProviderFactory providerFactory, IRegistryEditor registryEditor, IFileManager fileManager, string[] windowsServices)
        {
            _providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
            _registryEditor = registryEditor ?? throw new ArgumentNullException(nameof(registryEditor));
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
            _windowsServices = windowsServices ?? new string[0];
        }

        public string[] ListAllServices()
        {
            return _windowsServices;
        }

        public bool ServiceExists(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException(nameof(serviceName));
            using (var service = _providerFactory.GetWindowsServiceProvider(serviceName))
            {
                return service.ServiceExists();
            }
        }

        public string GetServiceDisplayName(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException(nameof(serviceName));
            using (var service = _providerFactory.GetWindowsServiceProvider(serviceName))
            {
                return service.DisplayName;
            }
        }

        public string GetServiceDllPath(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException(nameof(serviceName));

            return _registryEditor.ReadLocalMachineRegistryValue(_serviceRegistryPath + serviceName + @"\Parameters", "ServiceDll");
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
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException(nameof(serviceName));
            var serviceDllPath = GetServiceDllPath(serviceName);

            using (var service = _providerFactory.GetWindowsServiceProvider(serviceName))
            {
                return service.IsServiceEnabled()
                       && (string.IsNullOrEmpty(serviceDllPath) || _fileManager.Exists(serviceDllPath));
            }
        }

        public bool EnableService(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException(nameof(serviceName));
            RemoveWu10FromFileName(serviceName);
            var enabledRealtime = false;
            using (var service = _providerFactory.GetWindowsServiceProvider(serviceName))
            {
                if (!service.IsServiceRunAsLocalSystem())
                    service.SetAccountAsLocalSystem();
                enabledRealtime = service.TryEnableService();
                if (!enabledRealtime)
                {
                    SetStartModeViaRegistry(serviceName, ServiceStartMode.Manual);
                }
            }

            return enabledRealtime;
        }

        public void DisableService(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException(nameof(serviceName));
            using (var service = _providerFactory.GetWindowsServiceProvider(serviceName))
            {
                if (!service.TryDisableService())
                {
                    SetStartModeViaRegistry(serviceName, ServiceStartMode.Disabled);
                }
            }

            AddWu10ToFileName(serviceName);
        }

        public void AddWu10ToFileName(string serviceName)
        {
            var dllPath = GetServiceDllPath(serviceName);
            if (string.IsNullOrEmpty(dllPath))
                return;

            _fileManager.GiveOwnershipToAdministrators(dllPath);
            var wu10Path = GetPathWithWu10Prefix(dllPath);
            _fileManager.RenameFile(dllPath, wu10Path);
        }

        public void RemoveWu10FromFileName(string serviceName)
        {
            var dllPath = GetServiceDllPath(serviceName);
            if (string.IsNullOrEmpty(dllPath))
                return;

            var wu10Path = GetPathWithWu10Prefix(dllPath);

            // If the file has returned, then we need to assume that some other process has recreated
            // it.  It's safer to assume the new file is correct and delete the "old" file.
            if (_fileManager.Exists(dllPath))
            {
                _fileManager.Delete(wu10Path);
            }
            else
            {
                _fileManager.GiveOwnershipToAdministrators(wu10Path);
                _fileManager.RenameFile(wu10Path, dllPath);
                _fileManager.GiveOwnershipToTrustedInstaller(dllPath);
            }
        }

        private string GetPathWithWu10Prefix(string path)
        {
            var folder = _fileManager.GetDirectoryName(path);
            var fileName = _fileManager.GetFileName(path);
            if (fileName.StartsWith(_wu10FilePrefix, StringComparison.CurrentCultureIgnoreCase))
                return path;

            fileName = _wu10FilePrefix + fileName;
            var newPath = _fileManager.Combine(folder, fileName);
            return newPath;
        }

        private void SetStartModeViaRegistry(string serviceName, ServiceStartMode startMode)
        {
            _registryEditor.WriteLocalMachineRegistryDword(
                _serviceRegistryPath + serviceName,
                "Start",
                ((int)startMode).ToString());
        }
    }
}
