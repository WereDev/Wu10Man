using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WereDev.Utils.Wu10Man.Editors;
using WereDev.Utils.Wu10Man.Helpers;
using WereDev.Utils.Wu10Man.Interfaces;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for GroupPolicyControl.xaml
    /// </summary>
    public partial class GroupPolicyControl : UserControl
    {
        const string ENABLE = "ENABLE";
        const string DISABLE = "DISABLE";
        const string NOTIFY_DOWNLOAD = "NOTIFY_DOWNLOAD";
        const string NOTIFY_INSTALL = "NOTIFY_INSTALL";
        const string SCHEDULE_INSTALL = "SCHEDULE_INSTALL";

        const string AUOPTION_NOTIFY_DOWNLOAD = "2";
        const string AUOPTION_NOTIFY_INSTALL = "3";
        const string AUOPTION_SCHEDULE_INSTALL = "4";

        const string NOUPDATE_ENABLE = "0";
        const string NOUPDATE_DISABLE = "1";

        const string REGISTRY_ROOT = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU";
        const string REGISTRY_AUOPTION = "AuOptions";
        const string REGISTRY_NOUPDATE = "NoAutoUpdate";

        private IRegistryEditor RegistryEditor => DependencyManager.Resolve<IRegistryEditor>();

        public ObservableCollection<KeyValuePair<string, string>> PolicyOptions { get; set; }
        public KeyValuePair<string, string> SelectedPolicyOption { get; set; }

        public GroupPolicyControl()
        {
            Wu10Logger.LogInfo("Group Policy Control initializing.");
            CreatePolicyOptions();
            GetCurrentStatus();
            InitializeComponent();
            Wu10Logger.LogInfo("Group Policy Control initialized.");
        }

        private void CreatePolicyOptions()
        {
            var options = new ObservableCollection<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>( ENABLE, "Enable Automatic Updates" ),
                new KeyValuePair<string, string>( DISABLE, "Disable Automatic Updates" ),
                new KeyValuePair<string, string>( NOTIFY_DOWNLOAD, "Notify of Download and Installation" ),
                new KeyValuePair<string, string>( NOTIFY_INSTALL, "Automatic Download, Notify of Installation" ),
                
                //TODO: Implement scheduling config to get this working.
                //new KeyValuePair<string, string>( SCHEDULE_INSTALL, "Automatic Download, Schedule Install")
            };

            PolicyOptions = options;
        }

        private void GetCurrentStatus()
        {
            var status = GetNoUpdateStatus();
            if (status == ENABLE)
                status = GetAuOptionStatus();

            SelectedPolicyOption = PolicyOptions.FirstOrDefault(x => x.Key == status);
        }

        private string GetNoUpdateStatus()
        {
            var nauValue = RegistryEditor.ReadLocalMachineRegistryValue(REGISTRY_ROOT, REGISTRY_NOUPDATE);
            switch (nauValue)
            {
                case NOUPDATE_ENABLE:
                    return DISABLE;
                case NOUPDATE_DISABLE:
                default:
                    return ENABLE;
            }
        }

        private string GetAuOptionStatus()
        {
            var auValue = RegistryEditor.ReadLocalMachineRegistryValue(REGISTRY_ROOT, REGISTRY_AUOPTION);

            switch (auValue)
            {
                case AUOPTION_NOTIFY_DOWNLOAD:
                    return NOTIFY_DOWNLOAD;
                case AUOPTION_NOTIFY_INSTALL:
                    return NOTIFY_INSTALL;
                case AUOPTION_SCHEDULE_INSTALL:
                    return SCHEDULE_INSTALL;
                default:
                    return ENABLE;
            }
        }

        private void GroupPoliciesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPolicyOption = (KeyValuePair<string, string>)e.AddedItems[0];
        }

        private void SetGroupPolicy(object sender, System.Windows.RoutedEventArgs e)
        {
            switch (SelectedPolicyOption.Key)
            {
                case ENABLE:
                    RegistryEditor.DeleteLocalMachineRegistryValue(REGISTRY_ROOT, REGISTRY_AUOPTION);
                    RegistryEditor.DeleteLocalMachineRegistryValue(REGISTRY_ROOT, REGISTRY_NOUPDATE);
                    break;
                case DISABLE:
                    RegistryEditor.WriteLocalMachineRegistryValue(REGISTRY_ROOT, REGISTRY_NOUPDATE, NOUPDATE_ENABLE, RegistryValueKind.DWord);
                    RegistryEditor.DeleteLocalMachineRegistryValue(REGISTRY_ROOT, REGISTRY_AUOPTION);
                    break;
                case NOTIFY_DOWNLOAD:
                    RegistryEditor.WriteLocalMachineRegistryValue(REGISTRY_ROOT, REGISTRY_AUOPTION, AUOPTION_NOTIFY_DOWNLOAD, RegistryValueKind.DWord);
                    RegistryEditor.DeleteLocalMachineRegistryValue(REGISTRY_ROOT, REGISTRY_NOUPDATE);
                    break;
                case NOTIFY_INSTALL:
                    RegistryEditor.WriteLocalMachineRegistryValue(REGISTRY_ROOT, REGISTRY_AUOPTION, AUOPTION_NOTIFY_INSTALL, RegistryValueKind.DWord);
                    RegistryEditor.DeleteLocalMachineRegistryValue(REGISTRY_ROOT, REGISTRY_NOUPDATE);
                    break;
                case SCHEDULE_INSTALL:
                    RegistryEditor.WriteLocalMachineRegistryValue(REGISTRY_ROOT, REGISTRY_AUOPTION, AUOPTION_SCHEDULE_INSTALL, RegistryValueKind.DWord);
                    RegistryEditor.DeleteLocalMachineRegistryValue(REGISTRY_ROOT, REGISTRY_NOUPDATE);
                    break;
            }

            Wu10Logger.LogInfo(string.Format("Group Policy set: {0}", SelectedPolicyOption.Value));
            System.Windows.MessageBox.Show("Registry settings updated.", "Group Policies", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
