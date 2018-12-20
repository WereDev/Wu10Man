using System;
using System.ComponentModel;
using System.Windows.Controls;
using WereDev.Utils.Wu10Man.Editors;
using WereDev.Utils.Wu10Man.UserControls.Models;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for WindowsServicesControl.xaml
    /// </summary>
    public partial class WindowsServicesControl : UserControl
    {
        public const string UPDATE_SERVICE = "wuauserv";
        public const string MODULES_INSTALLER_SERVICE = "TrustedInstaller";

        readonly WindowsServicesModel _model;
        readonly Logger _logger;

        public WindowsServicesControl()
        {
            _model = new WindowsServicesModel();
            DataContext = _model;
            _logger = new Logger();            

            if (!DesignerProperties.GetIsInDesignMode(this))
                SetRuntimeOptions();
        }

        private void SetRuntimeOptions()
        {
            SetServiceStatus();
            InitializeComponent();
        }

        private void SetServiceStatus()
        {
            _model.IsUpdateServiceEnabled = IsServiceEnabled(UPDATE_SERVICE);
            _model.IsModulesInstallerServiceEnabled = IsServiceEnabled(MODULES_INSTALLER_SERVICE);
        }

        private bool IsServiceEnabled(string serviceName)
        {
            if (String.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            using (var service = new ServiceEditor(serviceName))
            {
                return service.IsServiceEnabled()
                       && service.IsServiceRunAsLocalSystem();
            }
        }

        private void EnableService(string serviceName)
        {
            if (String.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            using (var service = new ServiceEditor(serviceName))
            {
                service.EnableService();
                _logger.LogInfo(string.Format("Service enabled: {0}", serviceName));
            }
        }

        private void DisableService(string serviceName)
        {
            if (String.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            using (var service = new ServiceEditor(serviceName))
            {
                service.DisableService();
                _logger.LogInfo(string.Format("Service disabled: {0}", serviceName));
            }
        }

        private void BtnEnableDisable_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "btnWusEnable":
                    EnableService(UPDATE_SERVICE);
                    break;
                case "btnWusDisable":
                    DisableService(UPDATE_SERVICE);
                    break;
                case "btnWimsEnable":
                    EnableService(MODULES_INSTALLER_SERVICE);
                    break;
                case "btnWimsDisable":
                    DisableService(MODULES_INSTALLER_SERVICE);
                    break;
            }

            SetServiceStatus();

            System.Windows.MessageBox.Show("Windows service udpated.", "Windows Service", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
