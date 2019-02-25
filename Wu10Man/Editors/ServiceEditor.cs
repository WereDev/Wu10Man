using System;
using System.Security.Principal;
using System.ServiceProcess;

namespace WereDev.Utils.Wu10Man.Editors
{
    internal class ServiceEditor : IDisposable
    {
        private readonly ServiceController _serviceController;
        private readonly string _servicesRegistryPath = @"SYSTEM\CurrentControlSet\Services\";

        public ServiceEditor(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            _serviceController = new ServiceController(serviceName);
        }

        public void SetStartupType(ServiceStartMode startMode)
        {
            RegistryEditor.WriteLocalMachineRegistryValue(_servicesRegistryPath + _serviceController.ServiceName,
                                                          "Start",
                                                          ((int)startMode).ToString(),
                                                          Microsoft.Win32.RegistryValueKind.DWord);
        }

        public bool ServiceExists()
        {
            try
            {
                return !string.IsNullOrWhiteSpace(_serviceController.DisplayName);
            }
            catch
            {
                return false;
            }
        }

        public string DisplayName => _serviceController.DisplayName;

        public string GetServiceDLL()
        {
            return RegistryEditor.ReadLocalMachineRegistryValue(_servicesRegistryPath + _serviceController.ServiceName + @"\Parameters", "ServiceDll");
        }

        public void DisableService()
        {
            StopService();
            SetStartupType(ServiceStartMode.Disabled);
        }

        public void EnableService()
        {
            SetStartupType(ServiceStartMode.Manual);
        }

        public bool IsServiceEnabled()
        {
            return _serviceController.StartType != ServiceStartMode.Disabled;
        }

        public bool IsServiceRunAsLocalSystem()
        {
            var serviceUserName = ".\\" + ServiceCredentialsEditor.GetWindowsServiceUserName(_serviceController.ServiceName);

            return serviceUserName.Equals(ServiceCredentialsEditor.LOCAL_SYSTEM_USER, StringComparison.CurrentCultureIgnoreCase);
        }

        public void StopService()
        {
            if (_serviceController.Status != ServiceControllerStatus.Stopped)
            {
                _serviceController.Stop();
                _serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
            }
        }

        public void SetAccountAsLocalService()
        {

            ServiceCredentialsEditor.SetWindowsServiceCredentials(_serviceController.ServiceName, WellKnownSidType.LocalServiceSid);
        }

        public void SetAccountAsLocalSystem()
        {
            ServiceCredentialsEditor.SetWindowsServiceCredentials(_serviceController.ServiceName, ServiceCredentialsEditor.LOCAL_SYSTEM_USER, null);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _serviceController != null)
            {
                _serviceController.Dispose();
            }
        }
    }
}
