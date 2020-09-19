using System.Linq;
using System.Windows;
using WereDev.Utils.Wu10Man.Core;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.UserControls.Models;
using WPFSpark;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for HostsFileControl.xaml.
    /// </summary>
    public partial class HostsFileControl : UserControlBase<HostsFileModel>
    {
        private readonly IHostsFileEditor _hostsFileEditor;

        public HostsFileControl()
            : base()
        {
            _hostsFileEditor = DependencyManager.HostsFileEditor;
            TabTitle = "Hosts File";
            InitializeComponent();
        }

        protected override bool SetRuntimeOptions()
        {
            GetHostSettings();
            return true;
        }

        private void GetHostSettings()
        {
            var hostUrls = _hostsFileEditor.GetManagedHosts();
            if (hostUrls == null)
                return;

            var currentHosts = _hostsFileEditor.GetHostsInFile();
            var hostSettings = hostUrls.ToDictionary(x => x, x => !currentHosts.Contains(x));
            Model.HostStatus = hostSettings.Select(x => new HostStatus(x.Key, x.Value))
                                            .OrderBy(x => x.Host)
                                            .ToArray();
        }

        private void ToggleHostItem(object sender, RoutedEventArgs e)
        {
            if (!IsHostsFileLocked())
            {
                var toggle = (ToggleSwitch)sender;
                var kvp = (HostStatus)toggle.DataContext;
                if (toggle.IsChecked.Value)
                {
                    _hostsFileEditor.UnblockHostUrl(kvp.Host);
                    LogWriter.LogInfo($"Host UNBLOCKED: {kvp.Host}");
                }
                else
                {
                    _hostsFileEditor.BlockHostUrl(kvp.Host);
                    LogWriter.LogInfo($"Host BLOCKED: {kvp.Host}");
                }

                ShowUpdateNotice();
            }
        }

        private void UnblockAllHosts_Click(object sender, RoutedEventArgs e)
        {
            if (!IsHostsFileLocked())
            {
                _hostsFileEditor.SetHostsEntries(new string[0]);
                LogWriter.LogInfo($"All hosts UNBLOCKED");
                GetHostSettings();
                ShowUpdateNotice();
            }
        }

        private void BlockAllHosts_Click(object sender, RoutedEventArgs e)
        {
            if (!IsHostsFileLocked())
            {
                _hostsFileEditor.SetHostsEntries(_hostsFileEditor.GetManagedHosts());
                LogWriter.LogInfo($"All hosts BLOCKED");
                GetHostSettings();
                ShowUpdateNotice();
            }
        }

        private void ShowUpdateNotice()
        {
            MessageBox.Show("Hosts file updated.", TabTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool IsHostsFileLocked()
        {
            var lockingProcesses = _hostsFileEditor.GetLockingProcessNames();
            if (lockingProcesses?.Any() == true)
            {
                var processNames = string.Join("\r\n", lockingProcesses);
                var message = "The Hosts file is being locked by the following processes and cannot be updated:\r\n" + processNames;
                LogWriter.LogInfo(message);
                ShowWarningMessage(message);
                return true;
            }

            return false;
        }
    }
}
