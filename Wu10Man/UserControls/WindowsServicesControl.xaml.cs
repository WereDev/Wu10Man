using System;
using System.Linq;
using System.Windows;
using WereDev.Utils.Wu10Man.Core;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.UserControls.Models;
using WPFSpark;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for WindowsServicesControl.xaml.
    /// </summary>
    public partial class WindowsServicesControl : UserControlBaseWithWorker<WindowsServicesModel>
    {
        private readonly IWindowsServiceManager _windowsServiceManager;

        public WindowsServicesControl()
            : base()
        {
            _windowsServiceManager = DependencyManager.WindowsServiceManager;
            TabTitle = "Windows Services";
            InitializeComponent();
        }

        protected override bool SetRuntimeOptions()
        {
            BuildServiceStatus();
            return true;
        }

        private void BuildServiceStatus()
        {
            Model.Services = _windowsServiceManager.ListAllServices()
                                                   .Select(x => new WindowsServiceStatusModel(x))
                                                   .ToArray();
            foreach (var service in Model.Services)
                SetServiceStatus(service.ServiceName);
        }

        private void SetServiceStatus(string serviceName)
        {
            var serviceModel = Model.Services.Single(x => x.ServiceName == serviceName);
            serviceModel.ServiceExists = _windowsServiceManager.ServiceExists(serviceName);
            if (serviceModel.ServiceExists)
            {
                serviceModel.DisplayName = _windowsServiceManager.GetServiceDisplayName(serviceName);
                serviceModel.IsServiceEnabled = _windowsServiceManager.IsServiceEnabled(serviceName);
            }
        }

        private void ToggleService(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            var data = (WindowsServiceStatusModel)toggle.DataContext;
            if (toggle.IsChecked.Value)
            {
                var enabledRealtime = EnableService(data.ServiceName, data.DisplayName);
                var message = $"{data.DisplayName} has been ENABLED";
                if (!enabledRealtime)
                    message += "\r\rYou will need to reboot for the setting to take effect.";
                ShowInfoMessage(message);
            }
            else
            {
                DisableService(data.ServiceName, data.DisplayName);
                ShowInfoMessage($"{data.DisplayName} has been DISABLED");
            }
        }

        private void UpdateServices(object sender, RoutedEventArgs e)
        {
            var count = Model.Services.Where(x => x.ServiceExists).Count();
            RunBackgroundProcess(count, ToggleServices);
        }

        private bool EnableService(string serviceName, string displayName)
        {
            var enabledRealtime = _windowsServiceManager.EnableService(serviceName);
            SetServiceStatus(serviceName);
            LogWriter.LogInfo($"Service ENABLED: {serviceName} - {displayName}");
            return enabledRealtime;
        }

        private void DisableService(string serviceName, string displayName)
        {
            _windowsServiceManager.DisableService(serviceName);
            SetServiceStatus(serviceName);
            LogWriter.LogInfo($"Service DISABLED: {serviceName} - {displayName}");
        }

        private void ToggleServices(object sender, EventArgs e)
        {
            var allServicesDisabled = Model.AllServicesDisabled;
            var services = Model.Services.Where(x => x.ServiceExists).ToArray();
            foreach (var service in services)
            {
                if (allServicesDisabled)
                {
                    EnableService(service.ServiceName, service.DisplayName);
                }
                else if (service.IsServiceEnabled)
                {
                    DisableService(service.ServiceName, service.DisplayName);
                }

                AdvanceProgressBar();
            }

            BuildServiceStatus();
            ShowInfoMessage("Windows Services updated.");
        }
    }
}
