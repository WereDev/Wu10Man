using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
using WereDev.Utils.Wu10Man.Core;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.UserControls.Models;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for DeclutterControl.xaml.
    /// </summary>
    public partial class DeclutterControl : UserControl
    {
        private readonly ILogWriter _logWriter;
        private readonly IWindowsPackageManager _packageManager;
        private readonly DeclutterModel _model;

        public DeclutterControl()
        {
            _logWriter = DependencyManager.Resolve<ILogWriter>();
            _packageManager = DependencyManager.Resolve<IWindowsPackageManager>();
            _model = new DeclutterModel();

            _logWriter.LogInfo("Declutter Control initializing.");
            if (!DesignerProperties.GetIsInDesignMode(this))
                SetRuntimeOptions();
            _logWriter.LogInfo("Declutter Control initialized.");
        }

        private void SetRuntimeOptions()
        {
            GetPackageStatus();
            DataContext = _model;
            InitializeComponent();
        }

        private void GetPackageStatus()
        {
            var declutter = _packageManager.GetDeclutterConfig();
            var installedApps = _packageManager.ListInstalledPackages();
            var microsoftApps = _packageManager.MergePackageInfo(declutter.Microsoft, installedApps);
            var thirdPartyApps = _packageManager.MergePackageInfo(declutter.ThirdParty, installedApps);
            _model.MicrosoftPackages = microsoftApps.Select(x => new PackageInfo(x)).OrderBy(x => x.AppName).ToArray();
            _model.ThirdPartyPackages = thirdPartyApps.Select(x => new PackageInfo(x)).OrderBy(x => x.AppName).ToArray();
        }

        private void ShowMicrosoftApps(object sender, RoutedEventArgs e)
        {
            ListViewWindowsApps.ItemsSource = _model.MicrosoftPackages;
        }

        private void ShowThirdPartyApps(object sender, RoutedEventArgs e)
        {
            ListViewWindowsApps.ItemsSource = _model.ThirdPartyPackages;
        }

        private void RemoveCheckedApps(object sender, RoutedEventArgs e)
        {
            var packages = (PackageInfo[])ListViewWindowsApps.ItemsSource;
            var packagesToRemove = packages.Where(x => x.CheckedForRemoval).ToArray();

            foreach(var ptr in packagesToRemove)
            {
                _packageManager.RemovePackage(ptr.PackageName);
            }
            var s = "";
        }
    }
}
