using System.Runtime.InteropServices;

namespace WereDev.Utils.Win32Wrappers.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public class ServiceConfigInfo
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint ServiceType;
        [MarshalAs(UnmanagedType.U4)]
        public uint StartType;
        [MarshalAs(UnmanagedType.U4)]
        public uint ErrorControl;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string BinaryPathName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string LoadOrderGroup;
        [MarshalAs(UnmanagedType.U4)]
        public uint TagID;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Dependencies;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string ServiceStartName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string DisplayName;
    }
}
