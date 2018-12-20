using System.Windows;

namespace WereDev.Utils.Wu10Man
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        readonly Logger _logger;

        public App() : base()
        {
            _logger = new Logger();
            _logger.LogInfo("Application started");
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _logger.LogInfo("Application ended");
            base.OnExit(e);
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.LogError(e.Exception);
            string errorMessage = string.Format("{0}\r\n\r\nCheck the logs for more details.", e.Exception.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
