using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using WereDev.Utils.Wu10Man.Helpers;
using WereDev.Utils.Wu10Man.Interfaces;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for GroupPolicyControl.xaml.
    /// </summary>
    public partial class GroupPolicyControl : UserControl
    {
        private const string PolicyEnableUpdate = "ENABLE";
        private const string PolicyDisableUpdate = "DISABLE";
        private const string PolicyNotifyForDownload = "NOTIFY_DOWNLOAD";
        private const string PolicyNotifyForInstall = "NOTIFY_INSTALL";
        private const string PolicyScheduleInstall = "SCHEDULE_INSTALL";

        private const string AuOptionNotifyDownload = "2";
        private const string AuOptionNotifyInstall = "3";
        private const string AuOptionScheduleInstall = "4";

        private const string NoUpdateEnable = "0";
        private const string NoUpdateDisable = "1";

        private const string RegistryRoot = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU";
        private const string RegistryAuOptions = "AuOptions";
        private const string RegistryNoUpdate = "NoAutoUpdate";

        public GroupPolicyControl()
        {
            Wu10Logger.LogInfo("Group Policy Control initializing.");
            PolicyOptions = CreatePolicyOptions();
            GetCurrentStatus();
            InitializeComponent();
            Wu10Logger.LogInfo("Group Policy Control initialized.");
        }

        public ObservableCollection<KeyValuePair<string, string>> PolicyOptions { get; }

        public KeyValuePair<string, string> SelectedPolicyOption { get; set; }

        private IRegistryEditor RegistryEditor => DependencyManager.Resolve<IRegistryEditor>();

        private ObservableCollection<KeyValuePair<string, string>> CreatePolicyOptions()
        {
            var options = new ObservableCollection<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(PolicyEnableUpdate, "Enable Automatic Updates"),
                new KeyValuePair<string, string>(PolicyDisableUpdate, "Disable Automatic Updates"),
                new KeyValuePair<string, string>(PolicyNotifyForDownload, "Notify of Download and Installation"),
                new KeyValuePair<string, string>(PolicyNotifyForInstall, "Automatic Download, Notify of Installation"),
            };

            return options;
        }

        private void GetCurrentStatus()
        {
            var status = GetNoUpdateStatus();
            if (status == PolicyEnableUpdate)
                status = GetAuOptionStatus();

            SelectedPolicyOption = PolicyOptions.FirstOrDefault(x => x.Key == status);
        }

        private string GetNoUpdateStatus()
        {
            var nauValue = RegistryEditor.ReadLocalMachineRegistryValue(RegistryRoot, RegistryNoUpdate);
            switch (nauValue)
            {
                case NoUpdateEnable:
                    return PolicyDisableUpdate;
                case NoUpdateDisable:
                default:
                    return PolicyEnableUpdate;
            }
        }

        private string GetAuOptionStatus()
        {
            var registryValue = RegistryEditor.ReadLocalMachineRegistryValue(RegistryRoot, RegistryAuOptions);

            switch (registryValue)
            {
                case AuOptionNotifyDownload:
                    return PolicyNotifyForDownload;
                case AuOptionNotifyInstall:
                    return PolicyNotifyForInstall;
                case AuOptionScheduleInstall:
                    return PolicyScheduleInstall;
                default:
                    return PolicyEnableUpdate;
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
                case PolicyEnableUpdate:
                    RegistryEditor.DeleteLocalMachineRegistryValue(RegistryRoot, RegistryAuOptions);
                    RegistryEditor.DeleteLocalMachineRegistryValue(RegistryRoot, RegistryNoUpdate);
                    break;
                case PolicyDisableUpdate:
                    RegistryEditor.WriteLocalMachineRegistryValue(RegistryRoot, RegistryNoUpdate, NoUpdateEnable, RegistryValueKind.DWord);
                    RegistryEditor.DeleteLocalMachineRegistryValue(RegistryRoot, RegistryAuOptions);
                    break;
                case PolicyNotifyForDownload:
                    RegistryEditor.WriteLocalMachineRegistryValue(RegistryRoot, RegistryAuOptions, AuOptionNotifyDownload, RegistryValueKind.DWord);
                    RegistryEditor.DeleteLocalMachineRegistryValue(RegistryRoot, RegistryNoUpdate);
                    break;
                case PolicyNotifyForInstall:
                    RegistryEditor.WriteLocalMachineRegistryValue(RegistryRoot, RegistryAuOptions, AuOptionNotifyInstall, RegistryValueKind.DWord);
                    RegistryEditor.DeleteLocalMachineRegistryValue(RegistryRoot, RegistryNoUpdate);
                    break;
                case PolicyScheduleInstall:
                    RegistryEditor.WriteLocalMachineRegistryValue(RegistryRoot, RegistryAuOptions, AuOptionScheduleInstall, RegistryValueKind.DWord);
                    RegistryEditor.DeleteLocalMachineRegistryValue(RegistryRoot, RegistryNoUpdate);
                    break;
            }

            Wu10Logger.LogInfo(string.Format("Group Policy set: {0}", SelectedPolicyOption.Value));
            System.Windows.MessageBox.Show("Registry settings updated.", "Group Policies", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
