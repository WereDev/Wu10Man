using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using WereDev.Utils.Win32Wrappers;
using WereDev.Utils.Win32Wrappers.Models;
using WereDev.Utils.Wu10Man.Interfaces;
using WereDev.Utils.Wu10Man.Utilites.Models;

namespace WereDev.Utils.Wu10Man.Utilites
{
    // https://stackoverflow.com/questions/3876787/change-windows-service-password/3877268#3877268
    internal class ServiceCredentialsEditor : IServiceCredentialsEditor
    {
        public string GetUserName(WellKnownSidType sidType)
        {
            var sid = new SecurityIdentifier(sidType, WindowsIdentity.GetCurrent().User.AccountDomainSid);
            var account = sid.Translate(typeof(NTAccount));
            return account.Value;
        }

        public string GetWindowsServiceUserName(string serviceName)
        {
            IntPtr manager = IntPtr.Zero;
            IntPtr service = IntPtr.Zero;

            try
            {
                manager = ServiceWrapper.OpenSCManager(null, null, ServiceWrapper.SC_MANAGER_QUERY_ACESS);

                if (manager == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }

                service = ServiceWrapper.OpenService(manager, serviceName, ServiceWrapper.SERVICE_QUERY_CONFIG | ServiceWrapper.SERVICE_CHANGE_CONFIG);
                if (service == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }

                bool retCode = ServiceWrapper.QueryServiceConfig(service, IntPtr.Zero, 0, out uint bytesNeeded);
                if (!retCode && bytesNeeded == 0)
                    ThrowWin32Exception();

                IntPtr qscPtr = Marshal.AllocCoTaskMem(Convert.ToInt32(bytesNeeded));
                try
                {
                    retCode = ServiceWrapper.QueryServiceConfig(service, qscPtr, bytesNeeded, out bytesNeeded);
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
                    ServiceWrapper.CloseServiceHandle(service);
                if (manager != IntPtr.Zero)
                    ServiceWrapper.CloseServiceHandle(manager);
            }
        }

        public void SetWindowsServiceCredentials(string serviceName, WellKnownSidType sidType)
        {
            var username = GetUserName(sidType);
            SetWindowsServiceCredentials(serviceName, username, null);
        }

        public void SetWindowsServiceCredentials(string serviceName, string username, string password)
        {
            IntPtr manager = IntPtr.Zero;
            IntPtr service = IntPtr.Zero;
            try
            {
                manager = ServiceWrapper.OpenSCManager(null, null, ServiceWrapper.SC_MANAGER_ALL_ACCESS);
                if (manager == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }

                service = ServiceWrapper.OpenService(manager, serviceName, ServiceWrapper.SERVICE_QUERY_CONFIG | ServiceWrapper.SERVICE_CHANGE_CONFIG);
                if (service == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }

                if (!ServiceWrapper.ChangeServiceConfig(service, ServiceWrapper.SERVICE_NO_CHANGE, ServiceWrapper.SERVICE_NO_CHANGE, ServiceWrapper.SERVICE_NO_CHANGE, null, null, IntPtr.Zero, null, username, password, null))
                {
                    ThrowWin32Exception();
                }
            }
            finally
            {
                if (service != IntPtr.Zero)
                    ServiceWrapper.CloseServiceHandle(service);
                if (manager != IntPtr.Zero)
                    ServiceWrapper.CloseServiceHandle(manager);
            }
        }

        private void ThrowWin32Exception()
        {
            int error = Marshal.GetLastWin32Error();
            Win32Exception e = new Win32Exception(error);
            throw e;
        }
    }
}
