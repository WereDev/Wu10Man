using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WereDev.Utils.Wu10Man.Core;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.UserControls.Models;
using WPFSpark;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for WindowsServicesControl.xaml.
    /// </summary>
    public partial class WindowsServicesControl : UserControl, IDisposable
    {
        private const string TabTitle = "Windows Services";

        private readonly WindowsServicesModel _model = new WindowsServicesModel();
        private readonly ILogWriter _logWriter;
        private readonly IWindowsServiceManager _windowsServiceManager;
        private BackgroundWorker _worker;
        private bool _isDisposed = false;

        public WindowsServicesControl()
        {
            _logWriter = DependencyManager.LogWriter;
            _windowsServiceManager = DependencyManager.WindowsServiceManager;

            _logWriter.LogInfo("Windows Services initializing.");
            DataContext = _model;
        }

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (SetRuntimeOptions())
                    _logWriter.LogInfo("Windows Services initialized.");
            }

            base.OnRender(drawingContext);

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                _worker.Dispose();
            }

            _isDisposed = true;
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
                MessageBox.Show($"Error initializing {TabTitle} tab.", TabTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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
                var enabledRealtime = EnableService(data.ServiceName, data.DisplayName);
                var message = $"{data.DisplayName} has been ENABLED";
                if (!enabledRealtime)
                    message += "\r\rYou will need to reboot for the setting to take effect.";
                ShowMessage(message);
            }
            else
            {
                DisableService(data.ServiceName, data.DisplayName);
                ShowMessage($"{data.DisplayName} has been DISABLED");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "_worker part of larger scope.")]
        private void UpdateServices(object sender, RoutedEventArgs e)
        {
            InitializeProgressBar(0, _model.Services.Where(x => x.ServiceExists).Count());

            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
            };
            _worker.DoWork += ToggleServices;
            _worker.RunWorkerCompleted += RunWorkerCompleted;
            _worker.RunWorkerAsync();
        }

        private bool EnableService(string serviceName, string displayName)
        {
            var enabledRealtime = _windowsServiceManager.EnableService(serviceName);
            SetServiceStatus(serviceName);
            _logWriter.LogInfo($"Service ENABLED: {serviceName} - {displayName}");
            return enabledRealtime;
        }

        private void DisableService(string serviceName, string displayName)
        {
            _windowsServiceManager.DisableService(serviceName);
            SetServiceStatus(serviceName);
            _logWriter.LogInfo($"Service DISABLED: {serviceName} - {displayName}");
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(message, TabTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void ToggleServices(object sender, EventArgs e)
        {
            var allServicesDisabled = _model.AllServicesDisabled;
            var services = _model.Services.Where(x => x.ServiceExists).ToArray();
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
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BuildServiceStatus();
            ServicesListBox.ItemsSource = _model.Services;
            ShowMessage("Windows Services updated.");
            ServicesProgressBar.Visibility = Visibility.Hidden;
        }

        private void InitializeProgressBar(int minValue, int maxValue)
        {
            ServicesProgressBar.Visibility = Visibility.Visible;
            ServicesProgressBar.Initialize(minValue, maxValue);
        }

        private void AdvanceProgressBar()
        {
            ServicesProgressBar.Advance();
        }
    }
}
