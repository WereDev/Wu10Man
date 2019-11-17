using WereDev.Utils.Wu10Man.Interfaces;

namespace WereDev.Utils.Wu10Man.Helpers
{
    public static class EnvironmentVersionHelper
    {
        private const string WindowsVersionRegistryKey = @"SOFTWARE\WOW6432Node\Microsoft\Windows NT\CurrentVersion";
        private const string DotNetVersionRegistryKey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full";
        private static IRegistryEditor RegistryEditor => DependencyManager.Resolve<IRegistryEditor>();


        public static string GetDotNetFrameworkBuild()
        {
            var release = RegistryEditor.ReadLocalMachineRegistryValue(DotNetVersionRegistryKey, "Release");
            int.TryParse(release, out var releaseInt);

            if (releaseInt >= 528040)
                return $"{release} / 4.8 or later";
            else if (releaseInt >= 461808)
                return $"{release} / 4.7.2";
            else if (releaseInt >= 461308)
                return $"{release} / 4.7.1";
            else if (releaseInt >= 460798)
                return $"{release} / 4.7";
            else if (releaseInt >= 394802)
                return $"{release} / 4.6.2";
            else if (releaseInt >= 394254)
                return $"{release} / 4.6.1";
            else if (releaseInt >= 393295)
                return $"{release} / 4.6";
            else if (releaseInt >= 393273)
                return $"{release} / 4.6 RC";
            else if ((releaseInt >= 379893))
                return $"{release} / 4.5.2";
            else if ((releaseInt >= 378675))
                return $"{release} / 4.5.1";
            else if ((releaseInt >= 378389))
                return $"{release} / 4.5";
            else
                return $"{release} / No 4.5 or later version detected";
        }

        public static string GetWindowsVersion()
        {
            var windowsProduct = RegistryEditor.ReadLocalMachineRegistryValue(WindowsVersionRegistryKey, "ProductName");
            var windowsRelease = RegistryEditor.ReadLocalMachineRegistryValue(WindowsVersionRegistryKey, "ReleaseId");
            var windowsBuild = RegistryEditor.ReadLocalMachineRegistryValue(WindowsVersionRegistryKey, "CurrentBuild");
            var windowsBuildRevision = RegistryEditor.ReadLocalMachineRegistryValue(WindowsVersionRegistryKey, "BaseBuildRevisionNumber");
            return $"{windowsProduct} Version {windowsRelease} Build {windowsBuild}.{windowsBuildRevision}";
        }
    }
}
