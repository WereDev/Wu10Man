using System;
using System.Runtime.InteropServices;

namespace WereDev.Utils.Wu10Man.Utilites.Models
{
#pragma warning disable S1104 // Fields should not have public accessibility
    [StructLayout(LayoutKind.Sequential)]
    public class ServiceConfigInfo
    {
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 ServiceType;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 StartType;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 ErrorControl;
        [MarshalAs(UnmanagedType.LPWStr)]
        public String BinaryPathName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public String LoadOrderGroup;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 TagID;
        [MarshalAs(UnmanagedType.LPWStr)]
        public String Dependencies;
        [MarshalAs(UnmanagedType.LPWStr)]
        public String ServiceStartName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public String DisplayName;
    }
#pragma warning restore S1104 // Fields should not have public accessibility
}
