using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using WereDev.Utils.Wu10Man.Helpers;
using WereDev.Utils.Wu10Man.UserControls.Models;
using WPFSpark;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for HostsFileControl.xaml
    /// </summary>
    public partial class HostsFileControl : UserControl
    {
        private readonly HostsFileHelper _hostsFileHelper;
        private readonly HostsFileModel _model;

        public HostsFileControl()
        {
            _hostsFileHelper = new HostsFileHelper();
            _model = new HostsFileModel();
            if (!DesignerProperties.GetIsInDesignMode(this))
                SetRuntimeOptions();
        }

        private void SetRuntimeOptions()
        {
            GetHostSettings();
            DataContext = _model;
            InitializeComponent();
        }

        private void GetHostSettings()
        {
            var hostUrls = _hostsFileHelper.GetManagedHostUrls();
            if (hostUrls == null) return;
            var currentHosts = _hostsFileHelper.GetBlockedHostUrls();
            var hostSettings = hostUrls.ToDictionary(x => x, x => !currentHosts.Contains(x));
            _model.HostStatus = hostSettings.Select(x => new HostStatus(x.Key, x.Value))
                                            .OrderBy(x => x.Host)
                                            .ToArray();
        }

        private void ToggleHostItem(object sender, System.Windows.RoutedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            var kvp = (HostStatus)toggle.DataContext;
            if (toggle.IsChecked.Value)
            {
                _hostsFileHelper.UnblockHostUrl(kvp.Host);
                Wu10Logger.LogInfo($"Host UNBLOCKED: {kvp.Host}");
            }
            else
            {
                _hostsFileHelper.BlockHostUrl(kvp.Host);
                Wu10Logger.LogInfo($"Host BLOCKED: {kvp.Host}");
            }
            
            ShowUpdateNotice();
        }

        private void UnblockAllHosts_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _hostsFileHelper.UnblockAllHostUrls();
            Wu10Logger.LogInfo($"All hosts UNBLOCKED");
            GetHostSettings();
            ShowUpdateNotice();
        }

        private void BlockAllHosts_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _hostsFileHelper.BlockAllHostUrls();
            Wu10Logger.LogInfo($"All hosts BLOCKED");
            GetHostSettings();
            ShowUpdateNotice();
        }

        private void ShowUpdateNotice()
        {
            System.Windows.MessageBox.Show("Hosts file udpated.", "Hosts File", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
