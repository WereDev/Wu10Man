using Microsoft.Win32;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WereDev.Utils.Wu10Man.Helpers;
using WereDev.Utils.Wu10Man.Interfaces;
using WereDev.Utils.Wu10Man.UserControls.Models;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for PauseUpdates.xaml.
    /// </summary>
    public partial class PauseUpdatesControl : UserControl
    {
        private const string UXRegistryKey = @"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings";
        private const string DeferFeatureUpdatesPeriodInDays = "DeferFeatureUpdatesPeriodInDays";
        private const string PauseFeatureUpdatesStartTime = "PauseFeatureUpdatesStartTime";
        private const string PauseFeatureUpdatesEndTime = "PauseFeatureUpdatesEndTime";
        private const string DeferQualityUpdatesPeriodInDays = "DeferQualityUpdatesPeriodInDays";
        private const string PauseQualityUpdatesStartTime = "PauseQualityUpdatesStartTime";
        private const string PauseQualityUpdatesEndTime = "PauseQualityUpdatesEndTime";
        private const string PauseUpdatesExpiryTime = "PauseUpdatesExpiryTime";
        // private const string PendingRebootStartTime = "PendingRebootStartTime";

        private readonly Regex _numberOnlyRegex = new Regex("[^0-9]+");
        private readonly PauseUpdatesModel _model = new PauseUpdatesModel();

        public PauseUpdatesControl()
        {
            Wu10Logger.LogInfo("Pause and Defer initializing.");
            DataContext = _model;
            InitializeComponent();
            ReadCurrentSettings();
            Wu10Logger.LogInfo("Pause and Defer initialized.");
        }

        private IRegistryEditor RegistryEditor => DependencyManager.Resolve<IRegistryEditor>();

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _numberOnlyRegex.IsMatch(e.Text);
        }

        private void ReadCurrentSettings()
        {
            _model.FeatureUpdateDelayDays = GetIntFromRegistryValue(DeferFeatureUpdatesPeriodInDays);
            _model.FeatureUpdatePauseDate = GetDateFromRegistryValue(PauseFeatureUpdatesEndTime);
            _model.QualityUpdateDelayDays = GetIntFromRegistryValue(DeferQualityUpdatesPeriodInDays);
            _model.QualityUpdatePauseDate = GetDateFromRegistryValue(PauseQualityUpdatesEndTime);
        }

        private DateTime? GetDateFromRegistryValue(string registryName)
        {
            var registryValue = RegistryEditor.ReadLocalMachineRegistryValue(UXRegistryKey, registryName);
            if (DateTime.TryParse(registryValue, out var parsedValue))
                return parsedValue;
            else
                return null;
        }

        private int GetIntFromRegistryValue(string registryName)
        {
            var registryValue = RegistryEditor.ReadLocalMachineRegistryValue(UXRegistryKey, registryName);
            if (int.TryParse(registryValue, out var parsedValue))
                return parsedValue;
            else
                return 0;
        }

        private void SaveChanges(object sender, System.Windows.RoutedEventArgs e)
        {
            WriteChanges();
        }

        private void ClearValues(object sender, System.Windows.RoutedEventArgs e)
        {
            _model.FeatureUpdateDelayDays = 0;
            _model.FeatureUpdatePauseDate = null;
            _model.QualityUpdateDelayDays = 0;
            _model.QualityUpdatePauseDate = null;
            WriteChanges();
        }

        private void WriteChanges()
        {
            var nowString = GetDateString(DateTime.Now);
            if (_model.FeatureUpdatePauseDate.HasValue)
            {
                var pauseDateString = GetDateString(_model.FeatureUpdatePauseDate.Value);
                RegistryEditor.WriteLocalMachineRegistryValue(UXRegistryKey, PauseFeatureUpdatesStartTime, nowString, RegistryValueKind.String);
                RegistryEditor.WriteLocalMachineRegistryValue(UXRegistryKey, PauseFeatureUpdatesEndTime, pauseDateString, RegistryValueKind.String);
                Wu10Logger.LogInfo($"Saving Feature Pause Date: {pauseDateString}");
            }
            else
            {
                RegistryEditor.DeleteLocalMachineRegistryValue(UXRegistryKey, PauseFeatureUpdatesStartTime);
                RegistryEditor.DeleteLocalMachineRegistryValue(UXRegistryKey, PauseFeatureUpdatesEndTime);
                Wu10Logger.LogInfo("Removing Feature Pause Date");
            }

            if (_model.FeatureUpdateDelayDays > 0)
            {
                RegistryEditor.WriteLocalMachineRegistryValue(UXRegistryKey, DeferFeatureUpdatesPeriodInDays, _model.FeatureUpdateDelayDays.ToString(), RegistryValueKind.DWord);
                Wu10Logger.LogInfo($"Saving Feature Deferral Days: {_model.FeatureUpdateDelayDays}");
            }
            else
            {
                RegistryEditor.WriteLocalMachineRegistryValue(UXRegistryKey, DeferFeatureUpdatesPeriodInDays, "0", RegistryValueKind.DWord);
                Wu10Logger.LogInfo($"Saving Feature Deferral Days: 0");
            }

            if (_model.QualityUpdatePauseDate.HasValue)
            {
                var pauseDateString = GetDateString(_model.QualityUpdatePauseDate.Value);
                RegistryEditor.WriteLocalMachineRegistryValue(UXRegistryKey, PauseQualityUpdatesStartTime, nowString, RegistryValueKind.String);
                RegistryEditor.WriteLocalMachineRegistryValue(UXRegistryKey, PauseQualityUpdatesEndTime, pauseDateString, RegistryValueKind.String);
                Wu10Logger.LogInfo($"Saving Quality Pause Date: {pauseDateString}");
            }
            else
            {
                RegistryEditor.DeleteLocalMachineRegistryValue(UXRegistryKey, PauseQualityUpdatesStartTime);
                RegistryEditor.DeleteLocalMachineRegistryValue(UXRegistryKey, PauseQualityUpdatesEndTime);
                Wu10Logger.LogInfo("Removing Quality Pause Date");
            }

            if (_model.QualityUpdateDelayDays > 0)
            {
                RegistryEditor.WriteLocalMachineRegistryValue(UXRegistryKey, DeferQualityUpdatesPeriodInDays, _model.QualityUpdateDelayDays.ToString(), RegistryValueKind.DWord);
                Wu10Logger.LogInfo($"Saving Quality Deferral Days: {_model.QualityUpdateDelayDays}");
            }
            else
            {
                RegistryEditor.WriteLocalMachineRegistryValue(UXRegistryKey, DeferQualityUpdatesPeriodInDays, "0", RegistryValueKind.DWord);
                Wu10Logger.LogInfo($"Saving Quality Deferral Days: {_model.FeatureUpdateDelayDays}");
            }

            var latestDate = Math.Max(_model.FeatureUpdatePauseDate?.Ticks ?? 0, _model.QualityUpdatePauseDate?.Ticks ?? 0);
            if (latestDate > 0)
            {
                var pauseDateString = GetDateString(new DateTime(latestDate));
                RegistryEditor.WriteLocalMachineRegistryValue(UXRegistryKey, PauseUpdatesExpiryTime, GetDateString(new DateTime(latestDate)), RegistryValueKind.String);
                // RegistryEditor.WriteLocalMachineRegistryValue(UXRegistryKey, PendingRebootStartTime, now.ToString(dateFormat), RegistryValueKind.String);
                Wu10Logger.LogInfo($"Saving Pause Date Expiry: {pauseDateString}");
            }
            else
            {
                RegistryEditor.DeleteLocalMachineRegistryValue(UXRegistryKey, PauseUpdatesExpiryTime);
                Wu10Logger.LogInfo($"Removing Pause Date Expiry");
            }

            MessageBox.Show($"Windows Update pause dates and deferal period have been saved.", "Pause and Defer", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private string GetDateString(DateTime datetime)
        {
            var utc = datetime.Date.ToUniversalTime();
            var currentTime = DateTime.Now - DateTime.Today;
            var utcPlusTime = utc.Add(currentTime);
            return utcPlusTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }
    }
}
