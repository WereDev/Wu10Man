using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WereDev.Utils.Wu10Man.Core;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.Helpers;
using WereDev.Utils.Wu10Man.UserControls.Models;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for PauseUpdates.xaml.
    /// </summary>
    public partial class PauseUpdatesControl : UserControlBase<PauseUpdatesModel>
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
        private readonly IRegistryEditor _registryEditor;

        public PauseUpdatesControl()
            : base()
        {
            _registryEditor = DependencyManager.RegistryEditor;
            TabTitle = "Pause and Defer";
            InitializeComponent();
        }

        protected override bool SetRuntimeOptions()
        {
            Model.FeatureUpdateDelayDays = GetIntFromRegistryValue(DeferFeatureUpdatesPeriodInDays);
            Model.FeatureUpdatePauseDate = GetDateFromRegistryValue(PauseFeatureUpdatesEndTime);
            Model.QualityUpdateDelayDays = GetIntFromRegistryValue(DeferQualityUpdatesPeriodInDays);
            Model.QualityUpdatePauseDate = GetDateFromRegistryValue(PauseQualityUpdatesEndTime);
            return true;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _numberOnlyRegex.IsMatch(e.Text);
        }

        private DateTime? GetDateFromRegistryValue(string registryName)
        {
            var registryValue = _registryEditor.ReadLocalMachineRegistryValue(UXRegistryKey, registryName);
            if (DateTime.TryParse(registryValue, out var parsedValue))
                return parsedValue;
            else
                return null;
        }

        private int GetIntFromRegistryValue(string registryName)
        {
            var registryValue = _registryEditor.ReadLocalMachineRegistryValue(UXRegistryKey, registryName);
            if (int.TryParse(registryValue, out var parsedValue))
                return parsedValue;
            else
                return 0;
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            WriteChanges();
        }

        private void ClearValues(object sender, RoutedEventArgs e)
        {
            Model.FeatureUpdateDelayDays = 0;
            Model.FeatureUpdatePauseDate = null;
            Model.QualityUpdateDelayDays = 0;
            Model.QualityUpdatePauseDate = null;
            WriteChanges();
        }

        private void WriteChanges()
        {
            var nowString = GetDateString(DateTime.Now);
            if (Model.FeatureUpdatePauseDate.HasValue)
            {
                var pauseDateString = GetDateString(Model.FeatureUpdatePauseDate.Value);

                _registryEditor.WriteLocalMachineRegistryString(UXRegistryKey, PauseFeatureUpdatesStartTime, nowString);
                _registryEditor.WriteLocalMachineRegistryString(UXRegistryKey, PauseFeatureUpdatesEndTime, pauseDateString);
                LogWriter.LogInfo($"Saving Feature Pause Date: {pauseDateString}");
            }
            else
            {
                _registryEditor.DeleteLocalMachineRegistryValue(UXRegistryKey, PauseFeatureUpdatesStartTime);
                _registryEditor.DeleteLocalMachineRegistryValue(UXRegistryKey, PauseFeatureUpdatesEndTime);
                LogWriter.LogInfo("Removing Feature Pause Date");
            }

            if (Model.FeatureUpdateDelayDays > 0)
            {
                _registryEditor.WriteLocalMachineRegistryDword(UXRegistryKey, DeferFeatureUpdatesPeriodInDays, Model.FeatureUpdateDelayDays.ToString());
                LogWriter.LogInfo($"Saving Feature Deferral Days: {Model.FeatureUpdateDelayDays}");
            }
            else
            {
                _registryEditor.WriteLocalMachineRegistryDword(UXRegistryKey, DeferFeatureUpdatesPeriodInDays, "0");
                LogWriter.LogInfo($"Saving Feature Deferral Days: 0");
            }

            if (Model.QualityUpdatePauseDate.HasValue)
            {
                var pauseDateString = GetDateString(Model.QualityUpdatePauseDate.Value);
                _registryEditor.WriteLocalMachineRegistryString(UXRegistryKey, PauseQualityUpdatesStartTime, nowString);
                _registryEditor.WriteLocalMachineRegistryString(UXRegistryKey, PauseQualityUpdatesEndTime, pauseDateString);
                LogWriter.LogInfo($"Saving Quality Pause Date: {pauseDateString}");
            }
            else
            {
                _registryEditor.DeleteLocalMachineRegistryValue(UXRegistryKey, PauseQualityUpdatesStartTime);
                _registryEditor.DeleteLocalMachineRegistryValue(UXRegistryKey, PauseQualityUpdatesEndTime);
                LogWriter.LogInfo("Removing Quality Pause Date");
            }

            if (Model.QualityUpdateDelayDays > 0)
            {
                _registryEditor.WriteLocalMachineRegistryDword(UXRegistryKey, DeferQualityUpdatesPeriodInDays, Model.QualityUpdateDelayDays.ToString());
                LogWriter.LogInfo($"Saving Quality Deferral Days: {Model.QualityUpdateDelayDays}");
            }
            else
            {
                _registryEditor.WriteLocalMachineRegistryDword(UXRegistryKey, DeferQualityUpdatesPeriodInDays, "0");
                LogWriter.LogInfo($"Saving Quality Deferral Days: {Model.FeatureUpdateDelayDays}");
            }

            var latestDate = Math.Max(Model.FeatureUpdatePauseDate?.Ticks ?? 0, Model.QualityUpdatePauseDate?.Ticks ?? 0);
            if (latestDate > 0)
            {
                var pauseDateString = GetDateString(new DateTime(latestDate));
                _registryEditor.WriteLocalMachineRegistryString(UXRegistryKey, PauseUpdatesExpiryTime, GetDateString(new DateTime(latestDate)));
                LogWriter.LogInfo($"Saving Pause Date Expiry: {pauseDateString}");
            }
            else
            {
                _registryEditor.DeleteLocalMachineRegistryValue(UXRegistryKey, PauseUpdatesExpiryTime);
                LogWriter.LogInfo($"Removing Pause Date Expiry");
            }

            MessageBox.Show($"Windows Update pause dates and deferal period have been saved.", TabTitle, MessageBoxButton.OK, MessageBoxImage.Information);
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
