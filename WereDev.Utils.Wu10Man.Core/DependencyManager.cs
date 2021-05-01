using WereDev.Utils.Wu10Man.Core.Interfaces;

namespace WereDev.Utils.Wu10Man.Core
{
    public static class DependencyManager
    {
        public static ILogWriter LogWriter { get; set; }

        public static IFileManager FileManager { get; set; }

        public static IRegistryEditor RegistryEditor { get; set; }

        public static IWindowsPackageManager WindowsPackageManager { get; set; }

        public static IHostsFileEditor HostsFileEditor { get; set; }

        public static IWindowsServiceManager WindowsServiceManager { get; set; }

        public static IWindowsTaskManager WindowsTaskManager { get; set; }
    }
}
