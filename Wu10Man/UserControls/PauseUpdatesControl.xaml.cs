using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WereDev.Utils.Wu10Man.Editors;
using WereDev.Utils.Wu10Man.UserControls.Models;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for PauseUpdates.xaml
    /// </summary>
    public partial class PauseUpdatesControl : UserControl
    {
        private readonly Regex _numberOnlyRegex = new Regex("[^0-9]+");
        private readonly PauseUpdatesModel _model = new PauseUpdatesModel();
        private const string _registryKey = @"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings";
        private const string DeferFeatureUpdatesPeriodInDays = "DeferFeatureUpdatesPeriodInDays";
        private const string PauseFeatureUpdatesEndTime = "PauseFeatureUpdatesEndTime";
        private const string DeferQualityUpdatesPeriodInDays = "DeferQualityUpdatesPeriodInDays";
        private const string PauseQualityUpdatesEndTime = "PauseQualityUpdatesEndTime";

        public PauseUpdatesControl()
        {
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
            DataContext = _model;
            InitializeComponent();
            ReadCurrentSettings();
        }

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

        private DateTimeOffset? GetDateFromRegistryValue(string registryName)
        {
            var registryValue = RegistryEditor.ReadLocalMachineRegistryValue(_registryKey, registryName);
            if (DateTimeOffset.TryParse(registryValue, out var parsedValue))
                return parsedValue;
            else
                return null;
        }

        private int GetIntFromRegistryValue(string registryName)
        {
            var registryValue = RegistryEditor.ReadLocalMachineRegistryValue(_registryKey, registryName);
            if (int.TryParse(registryValue, out var parsedValue))
                return parsedValue;
            else
                return 0;
        }

        private void SaveChanges(object sender, System.Windows.RoutedEventArgs e)
        {
            
        }

        private void ResetChanges(object sender, System.Windows.RoutedEventArgs e)
        {
            ReadCurrentSettings();
        }

        private void ClearValues(object sender, System.Windows.RoutedEventArgs e)
        {
            _model.FeatureUpdateDelayDays = 0;
            _model.FeatureUpdatePauseDate = null;
            _model.QualityUpdateDelayDays = 0;
            _model.QualityUpdatePauseDate = null;
        }
    }
}
