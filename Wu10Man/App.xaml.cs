using System.Windows;

namespace WereDev.Utils.Wu10Man
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            Logger.LogInfo("Application started");
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Logger.LogInfo("Application ended");
            base.OnExit(e);
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.LogError(e.Exception);
            string errorMessage = string.Format("{0}\r\n\r\nCheck the logs for more details.", e.Exception.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
