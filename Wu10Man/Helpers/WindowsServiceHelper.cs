using System;
using System.IO;
using System.Linq;
using WereDev.Utils.Wu10Man.Editors;

namespace WereDev.Utils.Wu10Man.Helpers
{
    internal class WindowsServiceHelper
    {
        public const string UPDATE_SERVICE = "wuauserv";
        public const string MODULES_INSTALLER_SERVICE = "TrustedInstaller";
        public const string UPDATE_MEDIC_SERVICE = "WaaSMedicSvc";
        public const string SHOULD_NOT_EXIST = "ShouldNotExist";

        private readonly Wu10Logger _logger;
        private readonly FilesHelper _filesHelper;
        private readonly string _wu10FilePrefix = "Wu10Man_";

        public WindowsServiceHelper()
        {
            _logger = new Wu10Logger();
            _filesHelper = new FilesHelper();
        }

        public string[] ListAllServices()
        {
            return new string[]
            {
                UPDATE_SERVICE,
                MODULES_INSTALLER_SERVICE,
                SHOULD_NOT_EXIST,
                UPDATE_MEDIC_SERVICE
            };
        }

        public bool ServiceExists(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            using (var service = new ServiceEditor(serviceName))
            {
                return service.ServiceExists();
            }
        }

        public string GetServiceDisplayName(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            using (var service = new ServiceEditor(serviceName))
            {
                return service.DisplayName;
            }
        }

        public string GetServiceDllPath(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            using (var service = new ServiceEditor(serviceName))
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

            using (var service = new ServiceEditor(serviceName))
            {
                return service.IsServiceEnabled()
                       && (String.IsNullOrEmpty(serviceDllPath) || File.Exists(serviceDllPath));
            }
        }

        public void EnableService(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            RemoveWu10FromFileName(serviceName);
            using (var service = new ServiceEditor(serviceName))
            {
                service.EnableService();
                if (!service.IsServiceRunAsLocalSystem())
                    service.SetAccountAsLocalSystem();
            }
            _logger.LogInfo(string.Format("Service enabled: {0}", serviceName));
        }

        public void DisableService(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            using (var service = new ServiceEditor(serviceName))
            {
                service.DisableService();
            }
            AddWu10ToFileName(serviceName);
            _logger.LogInfo(string.Format("Service disabled: {0}", serviceName));
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

            // If the file has returned, then we need to assume that some other process has recreated
            // it.  It's safer to assume the new file is correct and delete the "old" file.
            if (File.Exists(dllPath))
            {
                File.Delete(dllPath);
            }
            else
            {
                var wu10Path = GetPathWithWu10Prefix(dllPath);
                _filesHelper.GiveOwnershipToAdministrators(wu10Path);
                _filesHelper.RenameFile(wu10Path, dllPath);
                _filesHelper.GiveOwnershipToTrustedInstaller(dllPath);
            }
        }

        private string GetPathWithWu10Prefix(string path)
        {
            var folder = Path.GetDirectoryName(path);
            var fileName = Path.GetFileName(path);
            if (fileName.StartsWith(_wu10FilePrefix, StringComparison.CurrentCultureIgnoreCase))
                return path;

            fileName = _wu10FilePrefix + fileName;
            var newPath = Path.Combine(folder, fileName);
            return newPath;
        }


    }
}
