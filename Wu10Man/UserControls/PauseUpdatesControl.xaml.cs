using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WereDev.Utils.Wu10Man.Core;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.Helpers;
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
        private const string TabTitle = "Pause and Defer";

        private readonly Regex _numberOnlyRegex = new Regex("[^0-9]+");
        private readonly PauseUpdatesModel _model = new PauseUpdatesModel();
        private readonly ILogWriter _logWriter;
        private readonly IRegistryEditor _registryEditor;

        public PauseUpdatesControl()
        {
            _logWriter = DependencyManager.LogWriter;
            _registryEditor = DependencyManager.RegistryEditor;

            _logWriter.LogInfo("Pause and Defer initializing.");
            DataContext = _model;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (SetRuntimeOptions())
                    _logWriter.LogInfo("Pause and Defer rendered.");
            }

            base.OnRender(drawingContext);

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _numberOnlyRegex.IsMatch(e.Text);
        }

        private bool SetRuntimeOptions()
        {
            try
            {
                _model.FeatureUpdateDelayDays = GetIntFromRegistryValue(DeferFeatureUpdatesPeriodInDays);
                _model.FeatureUpdatePauseDate = GetDateFromRegistryValue(PauseFeatureUpdatesEndTime);
                _model.QualityUpdateDelayDays = GetIntFromRegistryValue(DeferQualityUpdatesPeriodInDays);
                _model.QualityUpdatePauseDate = GetDateFromRegistryValue(PauseQualityUpdatesEndTime);
                return true;
            }
            catch (Exception ex)
            {
                _logWriter.LogError(ex);
                System.Windows.MessageBox.Show($"Error rendering {TabTitle} tab.", TabTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return false;
            }
            finally
            {
                InitializeComponent();
            }
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

                _registryEditor.WriteLocalMachineRegistryString(UXRegistryKey, PauseFeatureUpdatesStartTime, nowString);
                _registryEditor.WriteLocalMachineRegistryString(UXRegistryKey, PauseFeatureUpdatesEndTime, pauseDateString);
                _logWriter.LogInfo($"Saving Feature Pause Date: {pauseDateString}");
            }
            else
            {
                _registryEditor.DeleteLocalMachineRegistryValue(UXRegistryKey, PauseFeatureUpdatesStartTime);
                _registryEditor.DeleteLocalMachineRegistryValue(UXRegistryKey, PauseFeatureUpdatesEndTime);
                _logWriter.LogInfo("Removing Feature Pause Date");
            }

            if (_model.FeatureUpdateDelayDays > 0)
            {
                _registryEditor.WriteLocalMachineRegistryDword(UXRegistryKey, DeferFeatureUpdatesPeriodInDays, _model.FeatureUpdateDelayDays.ToString());
                _logWriter.LogInfo($"Saving Feature Deferral Days: {_model.FeatureUpdateDelayDays}");
            }
            else
            {
                _registryEditor.WriteLocalMachineRegistryDword(UXRegistryKey, DeferFeatureUpdatesPeriodInDays, "0");
                _logWriter.LogInfo($"Saving Feature Deferral Days: 0");
            }

            if (_model.QualityUpdatePauseDate.HasValue)
            {
                var pauseDateString = GetDateString(_model.QualityUpdatePauseDate.Value);
                _registryEditor.WriteLocalMachineRegistryString(UXRegistryKey, PauseQualityUpdatesStartTime, nowString);
                _registryEditor.WriteLocalMachineRegistryString(UXRegistryKey, PauseQualityUpdatesEndTime, pauseDateString);
                _logWriter.LogInfo($"Saving Quality Pause Date: {pauseDateString}");
            }
            else
            {
                _registryEditor.DeleteLocalMachineRegistryValue(UXRegistryKey, PauseQualityUpdatesStartTime);
                _registryEditor.DeleteLocalMachineRegistryValue(UXRegistryKey, PauseQualityUpdatesEndTime);
                _logWriter.LogInfo("Removing Quality Pause Date");
            }

            if (_model.QualityUpdateDelayDays > 0)
            {
                _registryEditor.WriteLocalMachineRegistryDword(UXRegistryKey, DeferQualityUpdatesPeriodInDays, _model.QualityUpdateDelayDays.ToString());
                _logWriter.LogInfo($"Saving Quality Deferral Days: {_model.QualityUpdateDelayDays}");
            }
            else
            {
                _registryEditor.WriteLocalMachineRegistryDword(UXRegistryKey, DeferQualityUpdatesPeriodInDays, "0");
                _logWriter.LogInfo($"Saving Quality Deferral Days: {_model.FeatureUpdateDelayDays}");
            }

            var latestDate = Math.Max(_model.FeatureUpdatePauseDate?.Ticks ?? 0, _model.QualityUpdatePauseDate?.Ticks ?? 0);
            if (latestDate > 0)
            {
                var pauseDateString = GetDateString(new DateTime(latestDate));
                _registryEditor.WriteLocalMachineRegistryString(UXRegistryKey, PauseUpdatesExpiryTime, GetDateString(new DateTime(latestDate)));
                _logWriter.LogInfo($"Saving Pause Date Expiry: {pauseDateString}");
            }
            else
            {
                _registryEditor.DeleteLocalMachineRegistryValue(UXRegistryKey, PauseUpdatesExpiryTime);
                _logWriter.LogInfo($"Removing Pause Date Expiry");
            }

            MessageBox.Show($"Windows Update pause dates and deferal period have been saved.", TabTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
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
