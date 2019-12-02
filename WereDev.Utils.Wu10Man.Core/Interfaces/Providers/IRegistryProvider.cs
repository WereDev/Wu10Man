using System.Security.Principal;

namespace WereDev.Utils.Wu10Man.Core.Interfaces.Providers
{
    public interface IRegistryProvider
    {
        string ReadLocalMachineRegistryValue(string registryKey, string registryName);

        void WriteLocalMachineRegistryDword(string registryKey, string registryName, string registryValue);

        void WriteLocalMachineRegistryString(string registryKey, string registryName, string registryValue);

        void DeleteLocalMachineRegistryValue(string registryKey, string registryName);

        void TakeLocalMachineOwnership(string registryKey, SecurityIdentifier user);

        void SetLocalMachineWritePermission(string registryKey, SecurityIdentifier user);
    }
}
