using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace WereDev.Utils.Wu10Man.Win32Wrappers
{
    internal static class WinApiWrapper
    {
        private const uint SE_PRIVILEGE_ENABLED = 2;

        /// <summary>
        ///     Enables or disables the specified privilege on the primary access token of the current process.</summary>
        /// <param name="privilege">
        ///     Privilege to enable or disable.</param>
        /// <param name="enable">
        ///     True to enable the privilege, false to disable it.</param>
        /// <returns>
        ///     True if the privilege was enabled prior to the change, false if it was disabled.</returns>
        public static bool ModifyPrivilege(PrivilegeName privilege, bool enable)
        {
            if (!LookupPrivilegeValue(null, privilege.ToString(), out Luid luid))
                throw new Win32Exception();

            using (var identity = WindowsIdentity.GetCurrent(TokenAccessLevels.AdjustPrivileges | TokenAccessLevels.Query))
            {
                var newPriv = new TokenPrivileges
                {
                    Privileges = new LuidAndAttributes[]
                    {
                        new LuidAndAttributes {
                            Luid = luid,
                            Attributes = enable ? SE_PRIVILEGE_ENABLED : 0
                        }
                    },
                    PrivilegeCount = 1
                };

                var prevPriv = new TokenPrivileges
                {
                    Privileges = new LuidAndAttributes[1],
                    PrivilegeCount = 1
                };

                if (!AdjustTokenPrivileges(identity.Token, false, ref newPriv, (uint)Marshal.SizeOf(prevPriv), ref prevPriv, out uint returnedBytes))
                    throw new Win32Exception();

                return prevPriv.PrivilegeCount == 0 ? enable /* didn't make a change */ : ((prevPriv.Privileges[0].Attributes & SE_PRIVILEGE_ENABLED) != 0);
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private extern static bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges, ref TokenPrivileges NewState,
           UInt32 BufferLengthInBytes, ref TokenPrivileges PreviousState, out UInt32 ReturnLengthInBytes);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private extern static bool LookupPrivilegeValue(string lpSystemName, string lpName, out Luid lpLuid);

        private struct TokenPrivileges
        {
            public UInt32 PrivilegeCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1 /*ANYSIZE_ARRAY*/)]
            public LuidAndAttributes[] Privileges;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LuidAndAttributes
        {
            public Luid Luid;
            public UInt32 Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Luid
        {
            public uint LowPart;
            public int HighPart;
        }
    }

    internal enum PrivilegeName
    {
        SeAssignPrimaryTokenPrivilege,
        SeAuditPrivilege,
        SeBackupPrivilege,
        SeChangeNotifyPrivilege,
        SeCreateGlobalPrivilege,
        SeCreatePagefilePrivilege,
        SeCreatePermanentPrivilege,
        SeCreateSymbolicLinkPrivilege,
        SeCreateTokenPrivilege,
        SeDebugPrivilege,
        SeEnableDelegationPrivilege,
        SeImpersonatePrivilege,
        SeIncreaseBasePriorityPrivilege,
        SeIncreaseQuotaPrivilege,
        SeIncreaseWorkingSetPrivilege,
        SeLoadDriverPrivilege,
        SeLockMemoryPrivilege,
        SeMachineAccountPrivilege,
        SeManageVolumePrivilege,
        SeProfileSingleProcessPrivilege,
        SeRelabelPrivilege,
        SeRemoteShutdownPrivilege,
        SeRestorePrivilege,
        SeSecurityPrivilege,
        SeShutdownPrivilege,
        SeSyncAgentPrivilege,
        SeSystemEnvironmentPrivilege,
        SeSystemProfilePrivilege,
        SeSystemtimePrivilege,
        SeTakeOwnershipPrivilege,
        SeTcbPrivilege,
        SeTimeZonePrivilege,
        SeTrustedCredManAccessPrivilege,
        SeUndockPrivilege,
        SeUnsolicitedInputPrivilege,
    }
}
