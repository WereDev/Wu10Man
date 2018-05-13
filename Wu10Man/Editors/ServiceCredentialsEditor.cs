using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WereDev.Utils.Wu10Man.Editors
{


    [StructLayout(LayoutKind.Sequential)]
    public class QUERY_SERVICE_CONFIG
    {
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
        public UInt32 dwServiceType;
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
        public UInt32 dwStartType;
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
        public UInt32 dwErrorControl;
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        public String lpBinaryPathName;
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        public String lpLoadOrderGroup;
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
        public UInt32 dwTagID;
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        public String lpDependencies;
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        public String lpServiceStartName;
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        public String lpDisplayName;
    };

    static class ServiceCredentialsEditor
    {

        private const uint SC_MANAGER_ALL_ACCESS = 0x000F003F;
        private const uint SERVICE_QUERY_CONFIG = 0x00001;
        private const uint SERVICE_CHANGE_CONFIG = 0x00002;
        private const uint SERVICE_NO_CHANGE = 0xffffffff;
        private const int ERROR_INSUFFICIENT_BUFFER = 122;

        public static void SetWindowsServiceCreds(string serviceName, string username, string password)
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

        public static void SetWindowsServiceCreds_LocalService(string serviceName)
        {
            SetWindowsServiceCreds(serviceName, @"NT AUTHORITY\LocalService", null);
        }

        private static void ThrowWin32Exception()
        {
            int error = Marshal.GetLastWin32Error();
            Win32Exception e = new Win32Exception(error);
            throw e;
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfig(IntPtr hService, UInt32 nServiceType, UInt32 nStartType, UInt32 nErrorControl, String lpBinaryPathName, String lpLoadOrderGroup, IntPtr lpdwTagId, String lpDependencies, String lpServiceStartName, String lpPassword, String lpDisplayName);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr OpenSCManager(
             string machineName,
             string databaseName,
             uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Boolean QueryServiceConfig(IntPtr hService, IntPtr intPtrQueryConfig, UInt32 cbBufSize, out UInt32 pcbBytesNeeded);

    }
}