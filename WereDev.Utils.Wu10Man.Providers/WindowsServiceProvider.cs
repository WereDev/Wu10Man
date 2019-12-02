using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using WereDev.Utils.Win32Wrappers;
using WereDev.Utils.Win32Wrappers.Models;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;

namespace WereDev.Utils.Wu10Man.Providers
{
    // https://stackoverflow.com/questions/3876787/change-windows-service-password/3877268#3877268
    public class WindowsServiceProvider : IWindowsServiceProvider
    {
        private ServiceController _serviceController;

        public const string UserNameLocalSystem = @".\LocalSystem";

        public WindowsServiceProvider(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
                throw new ArgumentNullException(nameof(serviceName));
            _serviceController = new ServiceController(serviceName);
        }

        public string DisplayName => _serviceController.DisplayName;

        public bool SetStartupType(ServiceStartMode startMode)
        {
            if (startMode == _serviceController.StartType)
                return true;

            // Doing this via Management Object allows for real-time service change
            // The new Windows Update Medic Service is pretty harsh on the access control
            bool doneRealtime = SetStartModeViaManagementObject(startMode);

            _serviceController.Refresh();
            return doneRealtime;
        }

        public bool ServiceExists()
        {
            try
            {
                return _serviceController.DisplayName != null;
            }
            catch
            {
                return false;
            }
        }

        public bool TryDisableService()
        {
            StopService();
            return SetStartupType(ServiceStartMode.Disabled);
        }

        public bool TryEnableService()
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
            SetWindowsServiceCredentials(_serviceController.ServiceName, WellKnownSidType.LocalServiceSid);
        }

        public void SetAccountAsLocalSystem()
        {
            SetWindowsServiceCredentials(_serviceController.ServiceName, UserNameLocalSystem, null);
        }

        public void SetWindowsServiceCredentials(string serviceName, WellKnownSidType sidType)
        {
            var sid = new SecurityIdentifier(sidType, WindowsIdentity.GetCurrent().User.AccountDomainSid);
            var account = sid.Translate(typeof(NTAccount));
            var username = account.Value;
            SetWindowsServiceCredentials(serviceName, username, null);
        }

        public void SetWindowsServiceCredentials(string serviceName, string username, string password)
        {
            IntPtr manager = IntPtr.Zero;
            IntPtr service = IntPtr.Zero;
            try
            {
                manager = WindowsServiceBridge.OpenSCManager(null, null, WindowsServiceBridge.SC_MANAGER_ALL_ACCESS);
                if (manager == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }

                service = WindowsServiceBridge.OpenService(manager, serviceName, WindowsServiceBridge.SERVICE_QUERY_CONFIG | WindowsServiceBridge.SERVICE_CHANGE_CONFIG);
                if (service == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }

                if (!WindowsServiceBridge.ChangeServiceConfig(service, WindowsServiceBridge.SERVICE_NO_CHANGE, WindowsServiceBridge.SERVICE_NO_CHANGE, WindowsServiceBridge.SERVICE_NO_CHANGE, null, null, IntPtr.Zero, null, username, password, null))
                {
                    ThrowWin32Exception();
                }
            }
            finally
            {
                if (service != IntPtr.Zero)
                    WindowsServiceBridge.CloseServiceHandle(service);
                if (manager != IntPtr.Zero)
                    WindowsServiceBridge.CloseServiceHandle(manager);
            }
        }

        public bool IsServiceRunAsLocalSystem()
        {
            var serviceUserName = ".\\" + GetWindowsServiceUserName();
            return serviceUserName.Equals(UserNameLocalSystem, StringComparison.OrdinalIgnoreCase);
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
                _serviceController = null;
            }
        }

        private bool SetStartModeViaManagementObject(ServiceStartMode startMode)
        {
            using (var mo = new ManagementObject(string.Format("Win32_Service.Name=\"{0}\"", _serviceController.ServiceName)))
            {
                var result = mo.InvokeMethod("ChangeStartMode", new object[] { startMode.ToString() });
                return result.ToString() != "2"; // Access Denied
            }
        }

        private void ThrowWin32Exception()
        {
            int error = Marshal.GetLastWin32Error();
            Win32Exception e = new Win32Exception(error);
            throw e;
        }

        private string GetWindowsServiceUserName()
        {
            IntPtr manager = IntPtr.Zero;
            IntPtr service = IntPtr.Zero;

            try
            {
                manager = WindowsServiceBridge.OpenSCManager(null, null, WindowsServiceBridge.SC_MANAGER_QUERY_ACESS);

                if (manager == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }

                service = WindowsServiceBridge.OpenService(manager, _serviceController.ServiceName, WindowsServiceBridge.SERVICE_QUERY_CONFIG | WindowsServiceBridge.SERVICE_CHANGE_CONFIG);
                if (service == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }

                bool retCode = WindowsServiceBridge.QueryServiceConfig(service, IntPtr.Zero, 0, out uint bytesNeeded);
                if (!retCode && bytesNeeded == 0)
                    ThrowWin32Exception();

                IntPtr qscPtr = Marshal.AllocCoTaskMem(Convert.ToInt32(bytesNeeded));
                try
                {
                    retCode = WindowsServiceBridge.QueryServiceConfig(service, qscPtr, bytesNeeded, out bytesNeeded);
                    if (!retCode)
                        ThrowWin32Exception();

                    var sci = (ServiceConfigInfo)Marshal.PtrToStructure(qscPtr, typeof(ServiceConfigInfo));
                    return sci.ServiceStartName;
                }
                finally
                {
                    Marshal.FreeCoTaskMem(qscPtr);
                }
            }
            finally
            {
                if (service != IntPtr.Zero)
                    WindowsServiceBridge.CloseServiceHandle(service);
                if (manager != IntPtr.Zero)
                    WindowsServiceBridge.CloseServiceHandle(manager);
            }
        }
    }
}
