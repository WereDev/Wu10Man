using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using WereDev.Utils.Wu10Man.Helpers;
using WereDev.Utils.Wu10Man.Interfaces;
using WereDev.Utils.Wu10Man.UserControls.Models;
using WPFSpark;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for WindowsServicesControl.xaml
    /// </summary>
    public partial class WindowsServicesControl : UserControl
    {
        private readonly WindowsServicesModel _model = new WindowsServicesModel();
        private IWindowsServiceManager ServiceManager => DependencyManager.Resolve<IWindowsServiceManager>();

        public WindowsServicesControl()
        {
            Wu10Logger.LogInfo("Windows Services initializing.");
            DataContext = _model;

            if (!DesignerProperties.GetIsInDesignMode(this))
                SetRuntimeOptions();
            Wu10Logger.LogInfo("Windows Services initialized.");
        }

        private void SetRuntimeOptions()
        {
            BuildServiceStatus();
            InitializeComponent();
        }

        private void BuildServiceStatus()
        {
            _model.Services = ServiceManager.ListAllServices()
                                                   .Select(x => new WindowsServiceStatusModel(x))
                                                   .ToArray();
            foreach (var service in _model.Services)
                SetServiceStatus(service.ServiceName);
        }

        private void SetServiceStatus(string serviceName)
        {
            var serviceModel = _model.Services.Single(x => x.ServiceName == serviceName);
            serviceModel.ServiceExists = ServiceManager.ServiceExists(serviceName);
            if (serviceModel.ServiceExists)
            {
                serviceModel.DisplayName = ServiceManager.GetServiceDisplayName(serviceName);
                serviceModel.IsServiceEnabled = ServiceManager.IsServiceEnabled(serviceName);
            }
        }

        private void ToggleService(object sender, System.Windows.RoutedEventArgs e)
        {
            var toggle = ((ToggleSwitch)sender);
            var data = (WindowsServiceStatusModel)toggle.DataContext;
            if (toggle.IsChecked.Value)
            {
                EnableService(data.ServiceName, data.DisplayName);
                Wu10Logger.LogInfo($"Service ENABLED: {data.ServiceName} - {data.DisplayName}");
            }
            else
            {
                DisableService(data.ServiceName, data.DisplayName);
                Wu10Logger.LogInfo($"Service DISABLED: {data.ServiceName} - {data.DisplayName}");
            }
        }

        private void EnableService(string serviceName, string displayName)
        {
            var enabledRealtime = ServiceManager.EnableService(serviceName);
            SetServiceStatus(serviceName);
            var message = $"{displayName} has been ENABLED";
            if (!enabledRealtime)
                message += "\r\rYou will need to reboot for the setting to take effect.";

            System.Windows.MessageBox.Show(message, "Windows Service", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void DisableService(string serviceName, string displayName)
        {
            ServiceManager.DisableService(serviceName);
            SetServiceStatus(serviceName);
            System.Windows.MessageBox.Show($"{displayName} has been DISABLED", "Windows Service", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
