using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using WereDev.Utils.Wu10Man.Interfaces;
using WereDev.Utils.Wu10Man.Utilites.Models;
using WereDev.Utils.Wu10Man.Win32Wrappers;

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
            IntPtr hManager = IntPtr.Zero;
            IntPtr hService = IntPtr.Zero;

            try
            {
                hManager = ServiceWrapper.OpenSCManager(null, null, ServiceWrapper.SC_MANAGER_QUERY_ACESS);

                if (hManager == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }
                hService = ServiceWrapper.OpenService(hManager, serviceName, ServiceWrapper.SERVICE_QUERY_CONFIG | ServiceWrapper.SERVICE_CHANGE_CONFIG);
                if (hService == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }

                bool retCode = ServiceWrapper.QueryServiceConfig(hService, IntPtr.Zero, 0, out uint bytesNeeded);
                if (!retCode && bytesNeeded == 0)
                    ThrowWin32Exception();

                IntPtr qscPtr = Marshal.AllocCoTaskMem(Convert.ToInt32(bytesNeeded));
                try
                {
                    retCode = ServiceWrapper.QueryServiceConfig(hService, qscPtr, bytesNeeded, out bytesNeeded);
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
                if (hService != IntPtr.Zero) ServiceWrapper.CloseServiceHandle(hService);
                if (hManager != IntPtr.Zero) ServiceWrapper.CloseServiceHandle(hManager);
            }
        }

        public void SetWindowsServiceCredentials(string serviceName, WellKnownSidType sidType)
        {
            var username = GetUserName(sidType);
            SetWindowsServiceCredentials(serviceName, username, null);
        }

        public void SetWindowsServiceCredentials(string serviceName, string username, string password)
        {
            IntPtr hManager = IntPtr.Zero;
            IntPtr hService = IntPtr.Zero;
            try
            {
                hManager = ServiceWrapper.OpenSCManager(null, null, ServiceWrapper.SC_MANAGER_ALL_ACCESS);
                if (hManager == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }
                hService = ServiceWrapper.OpenService(hManager, serviceName, ServiceWrapper.SERVICE_QUERY_CONFIG | ServiceWrapper.SERVICE_CHANGE_CONFIG);
                if (hService == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }

                if (!ServiceWrapper.ChangeServiceConfig(hService, ServiceWrapper.SERVICE_NO_CHANGE, ServiceWrapper.SERVICE_NO_CHANGE, ServiceWrapper.SERVICE_NO_CHANGE, null, null, IntPtr.Zero, null, username, password, null))
                {
                    ThrowWin32Exception();
                }
            }
            finally
            {
                if (hService != IntPtr.Zero) ServiceWrapper.CloseServiceHandle(hService);
                if (hManager != IntPtr.Zero) ServiceWrapper.CloseServiceHandle(hManager);
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