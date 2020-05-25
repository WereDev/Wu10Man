using System;
using System.Security;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;

namespace WereDev.Utils.Wu10Man.Core.Services
{
    public class RegistryEditor : IRegistryEditor
    {
        private readonly IWindowsApiProvider _windowsApiProvider;
        private readonly IRegistryProvider _registryProvider;
        private readonly IUserProvider _userProvider;

        public RegistryEditor(IWindowsApiProvider windowsApiProvider, IRegistryProvider registryProvider, IUserProvider userProvider)
        {
            _windowsApiProvider = windowsApiProvider ?? throw new ArgumentNullException(nameof(windowsApiProvider));
            _registryProvider = registryProvider ?? throw new ArgumentNullException(nameof(registryProvider));
            _userProvider = userProvider ?? throw new ArgumentNullException(nameof(userProvider));
        }

        public string ReadLocalMachineRegistryValue(string registryKey, string registryName)
        {
            if (string.IsNullOrWhiteSpace(registryKey))
                throw new ArgumentNullException(nameof(registryKey));
            if (string.IsNullOrWhiteSpace(registryName))
                throw new ArgumentNullException(nameof(registryName));

            return _registryProvider.ReadLocalMachineRegistryValue(registryKey, registryName);
        }

        public void WriteLocalMachineRegistryString(string registryKey, string registryName, string registryValue)
        {
            try
            {
                _registryProvider.WriteLocalMachineRegistryString(registryKey, registryName, registryValue);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException || ex is SecurityException)
                {
                    TakeOwnership(registryKey);
                    SetWritePermission(registryKey);
                    _registryProvider.WriteLocalMachineRegistryString(registryKey, registryName, registryValue);
                }
            }
        }

        public void WriteLocalMachineRegistryDword(string registryKey, string registryName, string registryValue)
        {
            try
            {
                _registryProvider.WriteLocalMachineRegistryDword(registryKey, registryName, registryValue);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException || ex is SecurityException)
                {
                    TakeOwnership(registryKey);
                    SetWritePermission(registryKey);
                    _registryProvider.WriteLocalMachineRegistryDword(registryKey, registryName, registryValue);
                }
            }
        }

        public void DeleteLocalMachineRegistryValue(string registryKey, string registryName)
        {
            try
            {
                _registryProvider.DeleteLocalMachineRegistryValue(registryKey, registryName);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException || ex is SecurityException)
                {
                    TakeOwnership(registryKey);
                    SetWritePermission(registryKey);
                    _registryProvider.DeleteLocalMachineRegistryValue(registryKey, registryName);
                }
            }
        }

        private void TakeOwnership(string registryKey)
        {
            try
            {
                _windowsApiProvider.ModifyPrivilege(Enums.WindowsApiPrivelegeNames.SeRestorePrivilege, true);
                _windowsApiProvider.ModifyPrivilege(Enums.WindowsApiPrivelegeNames.SeBackupPrivilege, true);
                _windowsApiProvider.ModifyPrivilege(Enums.WindowsApiPrivelegeNames.SeTakeOwnershipPrivilege, true);
                _registryProvider.TakeLocalMachineOwnership(registryKey, _userProvider.GetCurrentUser());
            }
            finally
            {
                _windowsApiProvider.ModifyPrivilege(Enums.WindowsApiPrivelegeNames.SeRestorePrivilege, false);
                _windowsApiProvider.ModifyPrivilege(Enums.WindowsApiPrivelegeNames.SeBackupPrivilege, false);
                _windowsApiProvider.ModifyPrivilege(Enums.WindowsApiPrivelegeNames.SeTakeOwnershipPrivilege, false);
            }
        }

        private void SetWritePermission(string registryKey)
        {
            _registryProvider.SetLocalMachineWritePermission(registryKey, _userProvider.GetCurrentUser());
        }
    }
}
