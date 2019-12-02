using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using WereDev.Utils.Wu10Man.Core;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.UserControls.Models;
using WPFSpark;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for HostsFileControl.xaml.
    /// </summary>
    public partial class HostsFileControl : UserControl
    {
        private readonly HostsFileModel _model;
        private readonly IHostsFileEditor _hostsFileEditor;
        private readonly ILogWriter _logWriter;

        public HostsFileControl()
        {
            _logWriter = DependencyManager.Resolve<ILogWriter>();
            _hostsFileEditor = DependencyManager.Resolve<IHostsFileEditor>();

            _logWriter.LogInfo("Hosts File initializing.");
            _model = new HostsFileModel();
            if (!DesignerProperties.GetIsInDesignMode(this))
                SetRuntimeOptions();
            _logWriter.LogInfo("Hosts File initialized.");
        }

        private void SetRuntimeOptions()
        {
            GetHostSettings();
            DataContext = _model;
            InitializeComponent();
        }

        private void GetHostSettings()
        {
            var hostUrls = _hostsFileEditor.GetManagedHosts();
            if (hostUrls == null)
                return;

            var currentHosts = _hostsFileEditor.GetHostsInFile();
            var hostSettings = hostUrls.ToDictionary(x => x, x => !currentHosts.Contains(x));
            _model.HostStatus = hostSettings.Select(x => new HostStatus(x.Key, x.Value))
                                            .OrderBy(x => x.Host)
                                            .ToArray();
        }

        private void ToggleHostItem(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!IsHostsFileLocked())
            {
                var toggle = (ToggleSwitch)sender;
                var kvp = (HostStatus)toggle.DataContext;
                if (toggle.IsChecked.Value)
                {
                    _hostsFileEditor.UnblockHostUrl(kvp.Host);
                    _logWriter.LogInfo($"Host UNBLOCKED: {kvp.Host}");
                }
                else
                {
                    _hostsFileEditor.BlockHostUrl(kvp.Host);
                    _logWriter.LogInfo($"Host BLOCKED: {kvp.Host}");
                }

                ShowUpdateNotice();
            }
        }

        private void UnblockAllHosts_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!IsHostsFileLocked())
            {
                _hostsFileEditor.SetHostsEntries(new string[0]);
                _logWriter.LogInfo($"All hosts UNBLOCKED");
                GetHostSettings();
                ShowUpdateNotice();
            }
        }

        private void BlockAllHosts_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!IsHostsFileLocked())
            {
                _hostsFileEditor.SetHostsEntries(_hostsFileEditor.GetManagedHosts());
                _logWriter.LogInfo($"All hosts BLOCKED");
                GetHostSettings();
                ShowUpdateNotice();
            }
        }

        private void ShowUpdateNotice()
        {
            System.Windows.MessageBox.Show("Hosts file updated.", "Hosts File", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private bool IsHostsFileLocked()
        {
            var lockingProcesses = _hostsFileEditor.GetLockingProcessNames();
            if (lockingProcesses?.Any() == true)
            {
                var processNames = string.Join("\r\n", lockingProcesses);
                var message = "The Hosts file is being locked by the following processes and cannot be updated:\r\n" + processNames;
                _logWriter.LogInfo(message);
                System.Windows.MessageBox.Show(message, "Hosts File Locked", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return true;
            }

            return false;
        }
    }
}
