using System.ComponentModel;
using System.Linq;
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
            BuildServiceStatus();
            InitializeComponent();
        }

        private void BuildServiceStatus()
        {
            _model.Services = _windowsServiceHelper.ListAllServices()
                                                   .Select(x => new WindowsServiceStatusModel(x))
                                                   .ToArray();
            foreach (var service in _model.Services)
                SetServiceStatus(service.ServiceName);
        }

        private void SetServiceStatus(string serviceName)
        {
            var serviceModel = _model.Services.Single(x => x.ServiceName == serviceName);
            serviceModel.ServiceExists = _windowsServiceHelper.ServiceExists(serviceName);
            if (serviceModel.ServiceExists)
            {
                serviceModel.DisplayName = _windowsServiceHelper.GetServiceDisplayName(serviceName);
                serviceModel.IsServiceEnabled = _windowsServiceHelper.IsServiceEnabled(serviceName);
            }
        }

        private void tglService_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var toggle = ((ToggleSwitch)sender);
            var data = (WindowsServiceStatusModel)toggle.DataContext;
            if (toggle.IsChecked.Value)
                EnableService(data.ServiceName, data.DisplayName);
            else
                DisableService(data.ServiceName, data.DisplayName);
        }

        private void EnableService(string serviceName, string displayName)
        {
            var enabledRealtime = _windowsServiceHelper.EnableService(serviceName);
            SetServiceStatus(serviceName);
            var message = $"{displayName} has been ENABLED";
            if (!enabledRealtime)
                message += "\r\rYou will need to reboot for the setting to take effect.";

            System.Windows.MessageBox.Show(message, "Windows Service", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void DisableService(string serviceName, string displayName)
        {
            _windowsServiceHelper.DisableService(serviceName);
            SetServiceStatus(serviceName);
            System.Windows.MessageBox.Show($"{displayName} has been DISABLED", "Windows Service", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }


    }
}
