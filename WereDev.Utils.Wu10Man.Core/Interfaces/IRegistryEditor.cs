namespace WereDev.Utils.Wu10Man.Core.Interfaces
{
    public interface IRegistryEditor
    {
        string ReadLocalMachineRegistryValue(string registryKey, string registryName);

        void WriteLocalMachineRegistryDword(string registryKey, string registryName, string registryValue);

        void WriteLocalMachineRegistryString(string registryKey, string registryName, string registryValue);

        void DeleteLocalMachineRegistryValue(string registryKey, string registryName);
    }
}
