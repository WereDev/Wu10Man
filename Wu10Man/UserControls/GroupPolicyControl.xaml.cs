using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Controls;
using WereDev.Utils.Wu10Man.Core;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.Helpers;

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

        private const string TabTitle = "Group Policies";

        private readonly ILogWriter _logWriter;
        private readonly IRegistryEditor _registryEditor;

        public GroupPolicyControl()
        {
            _logWriter = DependencyManager.LogWriter;
            _registryEditor = DependencyManager.RegistryEditor;

            _logWriter.LogInfo("Group Policy Control initializing.");

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (SetRuntimeOptions())
                    _logWriter.LogInfo("Group Policy Control initialized.");
            }
        }

        public ObservableCollection<KeyValuePair<string, string>> PolicyOptions { get; private set; }

        public KeyValuePair<string, string> SelectedPolicyOption { get; set; }

        private bool SetRuntimeOptions()
        {
            try
            {
                PolicyOptions = CreatePolicyOptions();
                GetCurrentStatus();
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
            var nauValue = _registryEditor.ReadLocalMachineRegistryValue(RegistryRoot, RegistryNoUpdate);
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
            var registryValue = _registryEditor.ReadLocalMachineRegistryValue(RegistryRoot, RegistryAuOptions);

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
                    _registryEditor.DeleteLocalMachineRegistryValue(RegistryRoot, RegistryAuOptions);
                    _registryEditor.DeleteLocalMachineRegistryValue(RegistryRoot, RegistryNoUpdate);
                    break;
                case PolicyDisableUpdate:
                    _registryEditor.WriteLocalMachineRegistryDword(RegistryRoot, RegistryNoUpdate, NoUpdateEnable);
                    _registryEditor.DeleteLocalMachineRegistryValue(RegistryRoot, RegistryAuOptions);
                    break;
                case PolicyNotifyForDownload:
                    _registryEditor.WriteLocalMachineRegistryDword(RegistryRoot, RegistryAuOptions, AuOptionNotifyDownload);
                    _registryEditor.DeleteLocalMachineRegistryValue(RegistryRoot, RegistryNoUpdate);
                    break;
                case PolicyNotifyForInstall:
                    _registryEditor.WriteLocalMachineRegistryDword(RegistryRoot, RegistryAuOptions, AuOptionNotifyInstall);
                    _registryEditor.DeleteLocalMachineRegistryValue(RegistryRoot, RegistryNoUpdate);
                    break;
                case PolicyScheduleInstall:
                    _registryEditor.WriteLocalMachineRegistryDword(RegistryRoot, RegistryAuOptions, AuOptionScheduleInstall);
                    _registryEditor.DeleteLocalMachineRegistryValue(RegistryRoot, RegistryNoUpdate);
                    break;
            }

            _logWriter.LogInfo(string.Format("Group Policy set: {0}", SelectedPolicyOption.Value));
            System.Windows.MessageBox.Show("Registry settings updated.", TabTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
