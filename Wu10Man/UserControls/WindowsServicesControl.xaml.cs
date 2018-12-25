using System.ComponentModel;
using System.Windows.Controls;
using WereDev.Utils.Wu10Man.Helpers;
using WereDev.Utils.Wu10Man.UserControls.Models;
using WPFSpark;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for WindowsServicesControl.xaml
    /// </summary>
    public partial class WindowsServicesControl : UserControl
    {
        private readonly WindowsServicesModel _model;
        private readonly WindowsServiceHelper _windowsServiceHelper;

        public WindowsServicesControl()
        {
            _model = new WindowsServicesModel();
            _windowsServiceHelper = new WindowsServiceHelper();
            DataContext = _model;

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
            _model.IsUpdateServiceEnabled = _windowsServiceHelper.IsServiceEnabled(WindowsServiceHelper.UPDATE_SERVICE);
            _model.IsModulesInstallerServiceEnabled = _windowsServiceHelper.IsServiceEnabled(WindowsServiceHelper.MODULES_INSTALLER_SERVICE);
        }

        private void tglWindowsUpdate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var isChecked = ((ToggleSwitch)sender).IsChecked.Value;
            if (isChecked)
                _windowsServiceHelper.EnableService(WindowsServiceHelper.UPDATE_SERVICE);
            else
                _windowsServiceHelper.DisableService(WindowsServiceHelper.UPDATE_SERVICE);

            SetServiceStatus();
            System.Windows.MessageBox.Show(string.Format("Windows Update service has been {0}.", isChecked ? "ENABLED" : "DISABLED"), "Windows Service", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void tglModulesInstaller_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var isChecked = ((ToggleSwitch)sender).IsChecked.Value;
            if (isChecked)
                _windowsServiceHelper.EnableService(WindowsServiceHelper.MODULES_INSTALLER_SERVICE);
            else
                _windowsServiceHelper.DisableService(WindowsServiceHelper.MODULES_INSTALLER_SERVICE);

            SetServiceStatus();
            System.Windows.MessageBox.Show(string.Format("Windows Modules Installer service has been {0}.", isChecked ? "ENABLED" : "DISABLED"), "Windows Service", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
