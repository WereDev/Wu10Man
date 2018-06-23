using System;
using System.Security.Principal;
using System.ServiceProcess;

namespace WereDev.Utils.Wu10Man.Editors
{
    class ServiceEditor : IDisposable
    {
        private readonly ServiceController _serviceController;


        public ServiceEditor(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            _serviceController = new ServiceController(serviceName);
        }

        public void SetStartupType(ServiceStartMode startMode)
        {
            RegistryEditor.WriteLocalMachineRegistryValue(@"SYSTEM\CurrentControlSet\Services\" + _serviceController.ServiceName,
                                                          "Start",
                                                          ((int)startMode).ToString(),
                                                          Microsoft.Win32.RegistryValueKind.DWord);
        }

        public void DisableService()
        {
            StopService();
            SetStartupType(ServiceStartMode.Disabled);
            SetAccountAsLocalService();
        }

        public void EnableService()
        {
            SetAccountAsLocalSystem();
            SetStartupType(ServiceStartMode.Manual);
        }

        public bool IsServiceEnabled()
        {
            return _serviceController.StartType != ServiceStartMode.Disabled;
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
