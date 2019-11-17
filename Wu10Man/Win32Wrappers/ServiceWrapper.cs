using System;
using System.Runtime.InteropServices;

namespace WereDev.Utils.Wu10Man.Win32Wrappers
{
    public static class ServiceWrapper
    {
        public const uint SC_MANAGER_ALL_ACCESS = 0x000F003F;
        public const uint SC_MANAGER_QUERY_ACESS = 0x10000000;
        public const uint SERVICE_NO_CHANGE = 0xffffffff; //this value is found in winsvc.h
        public const uint SERVICE_QUERY_CONFIG = 0x00000001;
        public const uint SERVICE_CHANGE_CONFIG = 0x00000002;
        //public const uint SERVICE_QUERY_STATUS = 0x00000004;
        //public const uint SERVICE_ENUMERATE_DEPENDENTS = 0x00000008;
        //public const uint SERVICE_START = 0x00000010;
        //public const uint SERVICE_STOP = 0x00000020;
        //public const uint SERVICE_PAUSE_CONTINUE = 0x00000040;
        //public const uint SERVICE_INTERROGATE = 0x00000080;
        //public const uint SERVICE_USER_DEFINED_CONTROL = 0x00000100;
        //public const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        //public const uint SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
        //                    SERVICE_CHANGE_CONFIG |
        //                    SERVICE_QUERY_STATUS |
        //                    SERVICE_ENUMERATE_DEPENDENTS |
        //                    SERVICE_START |
        //                    SERVICE_STOP |
        //                    SERVICE_PAUSE_CONTINUE |
        //                    SERVICE_INTERROGATE |
        //                    SERVICE_USER_DEFINED_CONTROL);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfig(IntPtr hService, UInt32 nServiceType, UInt32 nStartType, UInt32 nErrorControl, String lpBinaryPathName, String lpLoadOrderGroup, IntPtr lpdwTagId, String lpDependencies, String lpServiceStartName, String lpPassword, String lpDisplayName);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(
             string machineName,
             string databaseName,
             uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Boolean QueryServiceConfig(IntPtr hService, IntPtr intPtrQueryConfig, UInt32 cbBufSize, out UInt32 pcbBytesNeeded);
    }
}
