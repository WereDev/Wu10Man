using Autofac;
using System;
using System.Linq;
using System.Windows;
using WereDev.Utils.Wu10Man.Helpers;
using WereDev.Utils.Wu10Man.Interfaces;
using WereDev.Utils.Wu10Man.UserWindows;
using WereDev.Utils.Wu10Man.Utilites;
using WereDev.Utils.Wu10Man.Utilites.Models;

namespace WereDev.Utils.Wu10Man
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App : Application
    {
        public App()
            : base()
        {
            Wu10Logger.LogInfo("Application starting");
            try
            {
                RegisterDependencies();
                WriteStartupLogs();
                Dispatcher.UnhandledException += OnDispatcherUnhandledException;
                MainWindow = new MainWindow();
                MainWindow.Show();

                Wu10Logger.LogInfo("Application started");
            }
            catch (Exception ex)
            {
                Wu10Logger.LogError(ex);
                MessageBox.Show("An error occured attempting to initialize the application.  Check the log file for more details.", "Error!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Wu10Logger.LogInfo("Application ended");
            base.OnExit(e);
        }

        private void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.Register<IWindowsServices>((context, parameters) => { return GetWindowsServices(); });
            builder.RegisterType<FilesHelper>().As<IFilesHelper>();
            builder.RegisterType<HostsFileEditor>().As<IHostsFileEditor>();
            builder.RegisterType<RegistryEditor>().As<IRegistryEditor>();
            builder.RegisterType<ServiceCredentialsEditor>().As<IServiceCredentialsEditor>();
            builder.RegisterType<TokenEditor>().As<ITokenEditor>();
            builder.RegisterType<WindowsServiceManager>().As<IWindowsServiceManager>();
            builder.RegisterType<WindowsServiceProviderFactory>().As<IWindowsServiceProviderFactory>();

            DependencyManager.Container = builder.Build();
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Wu10Logger.LogError(e.Exception);
            string errorMessage = string.Format("{0}\r\n\r\nCheck the logs for more details.", e.Exception.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void WriteStartupLogs()
        {
            var appVersion = GetType().Assembly.GetName().Version;
            Wu10Logger.LogInfo($"Application version: v{appVersion.ToString()}");
            Wu10Logger.LogInfo(EnvironmentVersionHelper.GetWindowsVersion());
            Wu10Logger.LogInfo($".Net Framework: {EnvironmentVersionHelper.GetDotNetFrameworkBuild()}");
        }

        private WindowsServiceNames GetWindowsServices()
        {
            var serviceNames = System.Configuration.ConfigurationManager.AppSettings["WindowsServiceNames"];
            var split = serviceNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var names = new WindowsServiceNames();
            names.AddRange(split.Distinct());
            return names;
        }
    }
}
