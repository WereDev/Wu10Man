using System;
using System.Windows;
using WereDev.Utils.Wu10Man.Helpers;

namespace WereDev.Utils.Wu10Man
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            Wu10Logger.LogInfo("Application starting");
            try
            {
                WriteStartupLogs();
                this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
                this.MainWindow = new MainWindow();
                this.MainWindow.Show();

                Wu10Logger.LogInfo("Application started");
            }
            catch (Exception ex)
            {
                Wu10Logger.LogError(ex);
                MessageBox.Show("An error occured attempting to initialize the application.  Check the log file for more details.", "Error!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                this.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Wu10Logger.LogInfo("Application ended");
            base.OnExit(e);
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Wu10Logger.LogError(e.Exception);
            string errorMessage = string.Format("{0}\r\n\r\nCheck the logs for more details.", e.Exception.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        void WriteStartupLogs()
        {
            var appVersion = this.GetType().Assembly.GetName().Version;
            Wu10Logger.LogInfo($"Application version: v{appVersion.ToString()}");
            Wu10Logger.LogInfo(GetWindowsVersion());
            Wu10Logger.LogInfo($".Net Framework: {GetNetFrameworkBuild()}");
        }

        string GetWindowsVersion()
        {
            var windowsProduct = Editors.RegistryEditor.ReadLocalMachineRegistryValue(@"SOFTWARE\WOW6432Node\Microsoft\Windows NT\CurrentVersion", "ProductName");
            var windowsRelease = Editors.RegistryEditor.ReadLocalMachineRegistryValue(@"SOFTWARE\WOW6432Node\Microsoft\Windows NT\CurrentVersion", "ReleaseId");
            var windowsBuild = Editors.RegistryEditor.ReadLocalMachineRegistryValue(@"SOFTWARE\WOW6432Node\Microsoft\Windows NT\CurrentVersion", "CurrentBuild");
            var windowsBuildRevision = Editors.RegistryEditor.ReadLocalMachineRegistryValue(@"SOFTWARE\WOW6432Node\Microsoft\Windows NT\CurrentVersion", "BaseBuildRevisionNumber");
            return $"{windowsProduct} Version {windowsRelease} Build {windowsBuild}.{windowsBuildRevision}";
        }

        string GetNetFrameworkBuild()
        {
            var release = Editors.RegistryEditor.ReadLocalMachineRegistryValue(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full", "Release");
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
    }
}
