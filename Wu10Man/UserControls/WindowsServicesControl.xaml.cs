using System.Collections.Generic;
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
            _windowsServiceHelper.EnableService(serviceName);
            SetServiceStatus(serviceName);
            System.Windows.MessageBox.Show(string.Format("{0} has been ENABLED", displayName), "Windows Service", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void DisableService(string serviceName, string displayName)
        {
            _windowsServiceHelper.DisableService(serviceName);
            SetServiceStatus(serviceName);
            System.Windows.MessageBox.Show(string.Format("{0} has been DISABLED", displayName), "Windows Service", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }


    }
}
