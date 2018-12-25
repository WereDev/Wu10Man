using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace WereDev.Utils.Wu10Man.Editors
{
    // https://stackoverflow.com/questions/3876787/change-windows-service-password/3877268#3877268
    internal static class ServiceCredentialsEditor
    {

        private const uint SC_MANAGER_ALL_ACCESS = 0x000F003F;
        private const uint SC_MANAGER_QUERY_ACESS = 0x10000000;
        private const uint SERVICE_NO_CHANGE = 0xffffffff; //this value is found in winsvc.h
        private const uint SERVICE_QUERY_CONFIG = 0x00000001;
        private const uint SERVICE_CHANGE_CONFIG = 0x00000002;
        private const uint SERVICE_QUERY_STATUS = 0x00000004;
        private const uint SERVICE_ENUMERATE_DEPENDENTS = 0x00000008;
        private const uint SERVICE_START = 0x00000010;
        private const uint SERVICE_STOP = 0x00000020;
        private const uint SERVICE_PAUSE_CONTINUE = 0x00000040;
        private const uint SERVICE_INTERROGATE = 0x00000080;
        private const uint SERVICE_USER_DEFINED_CONTROL = 0x00000100;
        private const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        private const uint SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
                            SERVICE_CHANGE_CONFIG |
                            SERVICE_QUERY_STATUS |
                            SERVICE_ENUMERATE_DEPENDENTS |
                            SERVICE_START |
                            SERVICE_STOP |
                            SERVICE_PAUSE_CONTINUE |
                            SERVICE_INTERROGATE |
                            SERVICE_USER_DEFINED_CONTROL);

        public const string LOCAL_SYSTEM_USER = @".\LocalSystem";

        public static string GetUserName(WellKnownSidType sidType)
        {
            var sid = new SecurityIdentifier(sidType, null);
            var account = sid.Translate(typeof(NTAccount));
            return account.Value;
        }

        public static string GetWindowsServiceUserName(string serviceName)
        {
            IntPtr hManager = IntPtr.Zero;
            IntPtr hService = IntPtr.Zero;
            uint bytesNeeded;

            try
            {
                hManager = OpenSCManager(null, null, SC_MANAGER_QUERY_ACESS);

                if (hManager == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }
                hService = OpenService(hManager, serviceName, SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);
                if (hService == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }

                bool retCode = QueryServiceConfig(hService, IntPtr.Zero, 0, out bytesNeeded);
                if (!retCode && bytesNeeded == 0)
                    ThrowWin32Exception();

                IntPtr qscPtr = Marshal.AllocCoTaskMem(Convert.ToInt32(bytesNeeded));
                try
                {
                    retCode = QueryServiceConfig(hService, qscPtr, bytesNeeded, out bytesNeeded);
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
                if (hService != IntPtr.Zero) CloseServiceHandle(hService);
                if (hManager != IntPtr.Zero) CloseServiceHandle(hManager);
            }
        }

        public static void SetWindowsServiceCredentials(string serviceName, WellKnownSidType sidType)
        {
            var username = GetUserName(sidType);
            SetWindowsServiceCredentials(serviceName, username, null);
        }

        public static void SetWindowsServiceCredentials(string serviceName, string username, string password)
        {
            IntPtr hManager = IntPtr.Zero;
            IntPtr hService = IntPtr.Zero;
            try
            {
                hManager = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
                if (hManager == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }
                hService = OpenService(hManager, serviceName, SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);
                if (hService == IntPtr.Zero)
                {
                    ThrowWin32Exception();
                }

                if (!ChangeServiceConfig(hService, SERVICE_NO_CHANGE, SERVICE_NO_CHANGE, SERVICE_NO_CHANGE, null, null, IntPtr.Zero, null, username, password, null))
                {
                    ThrowWin32Exception();
                }
            }
            finally
            {
                if (hService != IntPtr.Zero) CloseServiceHandle(hService);
                if (hManager != IntPtr.Zero) CloseServiceHandle(hManager);
            }
        }

        private static void ThrowWin32Exception()
        {
            int error = Marshal.GetLastWin32Error();
            Win32Exception e = new Win32Exception(error);
            throw e;
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ChangeServiceConfig(IntPtr hService, UInt32 nServiceType, UInt32 nStartType, UInt32 nErrorControl, String lpBinaryPathName, String lpLoadOrderGroup, IntPtr lpdwTagId, String lpDependencies, String lpServiceStartName, String lpPassword, String lpDisplayName);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr OpenSCManager(
             string machineName,
             string databaseName,
             uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern Boolean QueryServiceConfig(IntPtr hService, IntPtr intPtrQueryConfig, UInt32 cbBufSize, out UInt32 pcbBytesNeeded);

    }
}