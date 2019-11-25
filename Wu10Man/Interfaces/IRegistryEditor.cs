using Microsoft.Win32;

namespace WereDev.Utils.Wu10Man.Interfaces
{
    public interface IRegistryEditor
    {
        string ReadLocalMachineRegistryValue(string registryKey, string registryName);

        void WriteLocalMachineRegistryValue(string registryKey, string registryName, string registryValue, RegistryValueKind registryValueKind);

        void DeleteLocalMachineRegistryValue(string registryKey, string registryName);
    }
}
