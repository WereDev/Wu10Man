using System;
using System.Linq;
using WereDev.Utils.Wu10Man.Editors;

namespace WereDev.Utils.Wu10Man.Helpers
{
    internal class WindowsServiceHelper
    {
        public const string UPDATE_SERVICE = "wuauserv";
        public const string MODULES_INSTALLER_SERVICE = "TrustedInstaller";
        private readonly Wu10Logger _logger;

        public WindowsServiceHelper()
        {
            _logger = new Wu10Logger();
        }

        public string[] ListAllServices()
        {
            return new string[]
            {
                UPDATE_SERVICE,
                MODULES_INSTALLER_SERVICE
            };
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
            using (var service = new ServiceEditor(serviceName))
            {
                return service.IsServiceEnabled()
                       && service.IsServiceRunAsLocalSystem();
            }
        }

        public void EnableService(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            using (var service = new ServiceEditor(serviceName))
            {
                service.EnableService();
                _logger.LogInfo(string.Format("Service enabled: {0}", serviceName));
            }
        }

        public void DisableService(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            using (var service = new ServiceEditor(serviceName))
            {
                service.DisableService();
                _logger.LogInfo(string.Format("Service disabled: {0}", serviceName));
            }
        }
    }
}
