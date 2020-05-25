using Microsoft.Win32;
using System;
using System.Security.AccessControl;
using System.Security.Principal;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;

namespace WereDev.Utils.Wu10Man.Providers
{
    public class RegistryProvider : IRegistryProvider
    {
        public string ReadLocalMachineRegistryValue(string registryKey, string registryName)
        {
            if (string.IsNullOrWhiteSpace(registryKey))
                throw new ArgumentNullException(nameof(registryKey));
            if (string.IsNullOrWhiteSpace(registryName))
                throw new ArgumentNullException(nameof(registryName));
            using (var regKey = Registry.LocalMachine.OpenSubKey(registryKey))
            {
                var regValue = regKey?.GetValue(registryName);
                return regValue?.ToString();
            }
        }

        public void WriteLocalMachineRegistryString(string registryKey, string registryName, string registryValue)
        {
            WriteRegistryValue(Registry.LocalMachine, registryKey, registryName, registryValue, RegistryValueKind.String);
        }

        public void WriteLocalMachineRegistryDword(string registryKey, string registryName, string registryValue)
        {
            WriteRegistryValue(Registry.LocalMachine, registryKey, registryName, registryValue, RegistryValueKind.DWord);
        }

        public void DeleteLocalMachineRegistryValue(string registryKey, string registryName)
        {
            DeleteRegistryValue(Registry.LocalMachine, registryKey, registryName);
        }

        public void TakeLocalMachineOwnership(string registryKey, SecurityIdentifier user)
        {
            TakeOwnership(Registry.LocalMachine, registryKey, user);
        }

        public void SetLocalMachineWritePermission(string registryKey, SecurityIdentifier user)
        {
            SetWritePermission(Registry.LocalMachine, registryKey, user);
        }

        private void WriteRegistryValue(RegistryKey registryRoot, string registryKey, string registryName, string registryValue, RegistryValueKind registryValueKind)
        {
            if (registryRoot == null)
                throw new ArgumentNullException(nameof(registryRoot));
            if (string.IsNullOrWhiteSpace(registryKey))
                throw new ArgumentNullException(nameof(registryKey));
            if (string.IsNullOrWhiteSpace(registryName))
                throw new ArgumentNullException(nameof(registryName));

            using (var regKey = OpenOrCreateRegistryKey(registryRoot, registryKey, true))
            {
                regKey.SetValue(registryName, registryValue, registryValueKind);
                regKey.Flush();
            }
        }

        private RegistryKey OpenOrCreateRegistryKey(RegistryKey registryRoot, string registryKey, bool writable)
        {
            var regKey = registryRoot.OpenSubKey(registryKey, writable);
            if (regKey == null)
                regKey = registryRoot.CreateSubKey(registryKey, writable);
            regKey.Flush();
            return regKey;
        }

        private void DeleteRegistryValue(RegistryKey registryRoot, string registryKey, string registryName)
        {
            if (registryRoot == null)
                throw new ArgumentNullException(nameof(registryRoot));
            if (string.IsNullOrWhiteSpace(registryKey))
                throw new ArgumentNullException(nameof(registryKey));
            if (string.IsNullOrWhiteSpace(registryName))
                throw new ArgumentNullException(nameof(registryName));

            using (var regKey = registryRoot.OpenSubKey(registryKey, false))
            {
                var value = regKey.GetValue(registryName);
                if (value == null)
                    return;
            }

            using (var regKey = registryRoot.OpenSubKey(registryKey, true))
            {
                regKey.DeleteValue(registryName);
            }
        }

        private void TakeOwnership(RegistryKey registryRoot, string registryKey, SecurityIdentifier user)
        {
            using (var regKey = registryRoot.OpenSubKey(registryKey, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.TakeOwnership))
            {
                var regSec = regKey.GetAccessControl();
                regSec.SetOwner(user);
                regKey.SetAccessControl(regSec);
            }
        }

        private void SetWritePermission(RegistryKey registryRoot, string registryKey, SecurityIdentifier user)
        {
            using (var regKey = registryRoot.OpenSubKey(registryKey, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ChangePermissions))
            {
                var regSec = regKey.GetAccessControl();

                RegistryAccessRule regRule = new RegistryAccessRule(
                    user,
                    RegistryRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow);

                regSec.AddAccessRule(regRule);
                regKey.SetAccessControl(regSec);
            }
        }
    }
}
