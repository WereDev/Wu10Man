using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using WereDev.Utils.Wu10Man.Core;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.Helpers;
using WereDev.Utils.Wu10Man.UserControls.Models;
using WPFSpark;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for WindowsServicesControl.xaml.
    /// </summary>
    public partial class WindowsServicesControl : UserControl
    {
        private const string TabTitle = "Windows Services";

        private readonly WindowsServicesModel _model = new WindowsServicesModel();
        private readonly ILogWriter _logWriter;
        private readonly IWindowsServiceManager _windowsServiceManager;

        public WindowsServicesControl()
        {
            _logWriter = DependencyManager.LogWriter;
            _windowsServiceManager = DependencyManager.WindowsServiceManager;

            _logWriter.LogInfo("Windows Services initializing.");
            DataContext = _model;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (SetRuntimeOptions())
                    _logWriter.LogInfo("Windows Services initialized.");
            }
        }

        private bool SetRuntimeOptions()
        {
            try
            {
                BuildServiceStatus();
                return true;
            }
            catch (Exception ex)
            {
                _logWriter.LogError(ex);
                System.Windows.MessageBox.Show($"Error initializing {TabTitle} tab.", TabTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return false;
            }
            finally
            {
                InitializeComponent();
            }
        }

        private void BuildServiceStatus()
        {
            _model.Services = _windowsServiceManager.ListAllServices()
                                                   .Select(x => new WindowsServiceStatusModel(x))
                                                   .ToArray();
            foreach (var service in _model.Services)
                SetServiceStatus(service.ServiceName);
        }

        private void SetServiceStatus(string serviceName)
        {
            var serviceModel = _model.Services.Single(x => x.ServiceName == serviceName);
            serviceModel.ServiceExists = _windowsServiceManager.ServiceExists(serviceName);
            if (serviceModel.ServiceExists)
            {
                serviceModel.DisplayName = _windowsServiceManager.GetServiceDisplayName(serviceName);
                serviceModel.IsServiceEnabled = _windowsServiceManager.IsServiceEnabled(serviceName);
            }
        }

        private void ToggleService(object sender, System.Windows.RoutedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            var data = (WindowsServiceStatusModel)toggle.DataContext;
            if (toggle.IsChecked.Value)
            {
                EnableService(data.ServiceName, data.DisplayName);
                _logWriter.LogInfo($"Service ENABLED: {data.ServiceName} - {data.DisplayName}");
            }
            else
            {
                DisableService(data.ServiceName, data.DisplayName);
                _logWriter.LogInfo($"Service DISABLED: {data.ServiceName} - {data.DisplayName}");
            }
        }

        private void EnableService(string serviceName, string displayName)
        {
            var enabledRealtime = _windowsServiceManager.EnableService(serviceName);
            SetServiceStatus(serviceName);
            var message = $"{displayName} has been ENABLED";
            if (!enabledRealtime)
                message += "\r\rYou will need to reboot for the setting to take effect.";

            System.Windows.MessageBox.Show(message, TabTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void DisableService(string serviceName, string displayName)
        {
            _windowsServiceManager.DisableService(serviceName);
            SetServiceStatus(serviceName);
            System.Windows.MessageBox.Show($"{displayName} has been DISABLED", TabTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
