using System;
using System.ComponentModel;
using System.Management;
using System.Security.Principal;
using System.ServiceProcess;
using WereDev.Utils.Wu10Man.Helpers;
using WereDev.Utils.Wu10Man.Interfaces;

namespace WereDev.Utils.Wu10Man.Utilites
{
    internal class WindowsServiceProvider : IDisposable, Interfaces.IWindowsServiceProvider
    {
        private ServiceController _serviceController;
        private readonly string _servicesRegistryPath = @"SYSTEM\CurrentControlSet\Services\";
        private readonly IRegistryEditor _registryEditor;
        private readonly IServiceCredentialsEditor _serviceCredentialsEditor;

        public WindowsServiceProvider(string serviceName, IRegistryEditor registryEditor, IServiceCredentialsEditor serviceCredentialsEditor)
        {
            if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            _serviceController = new ServiceController(serviceName);
            _registryEditor = registryEditor ?? throw new ArgumentNullException(nameof(registryEditor));
            _serviceCredentialsEditor = serviceCredentialsEditor ?? throw new ArgumentNullException(nameof(serviceCredentialsEditor));
        }

        public bool SetStartupType(ServiceStartMode startMode)
        {
            if (startMode == _serviceController.StartType) return true;

            //Doing this via Management Object allows for real-time service change
            //The new Windows Update Medic Service is pretty harsh on the access control
            bool doneRealtime = SetStartModeViaManagementObject(startMode);
            if (!doneRealtime)
            {
                //If this is done via registry, then a reboot is required
                SetStartModeViaRegistry(startMode);
            }
            _serviceController.Refresh();
            return doneRealtime;
        }

        public bool ServiceExists()
        {
            try
            {
                return _serviceController.DisplayName != null;
            }
            catch(Exception ex)
            {
                Wu10Logger.LogError(ex);
                return false;
            }
        }

        public string DisplayName => _serviceController.DisplayName;

        public string GetServiceDLL()
        {
            return _registryEditor.ReadLocalMachineRegistryValue(_servicesRegistryPath + _serviceController.ServiceName + @"\Parameters", "ServiceDll");
        }

        public void DisableService()
        {
            StopService();
            SetStartupType(ServiceStartMode.Disabled);
        }

        public bool EnableService()
        {
            return SetStartupType(ServiceStartMode.Manual);
        }

        public bool IsServiceEnabled()
        {
            try
            {
                return _serviceController.StartType != ServiceStartMode.Disabled;
            }
            catch (Win32Exception)
            {
                // Some .Net version seem to throw an exception at this point, but it is
                // sort of a .Net thing because of file renames, so I can assume false.
                return false;
            }
        }

        public bool IsServiceRunAsLocalSystem()
        {
            var serviceUserName = ".\\" + _serviceCredentialsEditor.GetWindowsServiceUserName(_serviceController.ServiceName);

            return serviceUserName.Equals(Constants.USERNAME_LOCAL_SYSTEM, StringComparison.CurrentCultureIgnoreCase);
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

            _serviceCredentialsEditor.SetWindowsServiceCredentials(_serviceController.ServiceName, WellKnownSidType.LocalServiceSid);
        }

        public void SetAccountAsLocalSystem()
        {
            _serviceCredentialsEditor.SetWindowsServiceCredentials(_serviceController.ServiceName, Constants.USERNAME_LOCAL_SYSTEM, null);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool SetStartModeViaManagementObject(ServiceStartMode startMode)
        {
            using (var mo = new ManagementObject(string.Format("Win32_Service.Name=\"{0}\"", _serviceController.ServiceName)))
            {
                var result = mo.InvokeMethod("ChangeStartMode", new object[] { startMode.ToString() });
                return result.ToString() != "2"; //Access Denied
            }
        }

        private void SetStartModeViaRegistry(ServiceStartMode startMode)
        {
            _registryEditor.WriteLocalMachineRegistryValue(_servicesRegistryPath + _serviceController.ServiceName,
                                                          "Start",
                                                          ((int)startMode).ToString(),
                                                          Microsoft.Win32.RegistryValueKind.DWord);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _serviceController != null)
            {
                _serviceController.Dispose();
                _serviceController = null;
            }
        }
    }
}
