using System;
using System.Linq;
using System.Threading;
using System.Windows;
using WereDev.Utils.Wu10Man.Core;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.UserControls.Models;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for DeclutterControl.xaml.
    /// </summary>
    public partial class DeclutterControl : UserControlBaseWithWorker<DeclutterModel>
    {
        private readonly IWindowsPackageManager _packageManager;

        public DeclutterControl()
            : base()
        {
            _packageManager = DependencyManager.WindowsPackageManager;
            TabTitle = "Declutter";

            InitializeComponent();
        }

        protected override bool SetRuntimeOptions()
        {
            var declutter = _packageManager.GetDeclutterConfig();
            var installedApps = _packageManager.ListInstalledPackages();
            var microsoftApps = _packageManager.MergePackageInfo(declutter.Microsoft, installedApps);
            Model.SetPackages(DeclutterModel.PackageSource.Microsoft, microsoftApps.Select(x => new PackageInfo(x)));
            var thirdPartyApps = _packageManager.MergePackageInfo(declutter.ThirdParty, installedApps);
            Model.SetPackages(DeclutterModel.PackageSource.ThirdParty, thirdPartyApps.Select(x => new PackageInfo(x)));
            return true;
        }

        private void ShowMicrosoftApps(object sender, RoutedEventArgs e)
        {
            Model.ActiveSource = DeclutterModel.PackageSource.Microsoft;
        }

        private void ShowThirdPartyApps(object sender, RoutedEventArgs e)
        {
            Model.ActiveSource = DeclutterModel.PackageSource.ThirdParty;
        }

        private void ToggleAppsSelection(object sender, RoutedEventArgs e)
        {
            var setChecked = !Model.AllPackagesSelected;

            foreach (var package in Model.Packages)
            {
                if (package.CheckedForRemoval != setChecked)
                {
                    package.CheckedForRemoval = setChecked;
                }
            }
        }

        private void RemoveCheckedApps(object sender, RoutedEventArgs e)
        {
            var packages = (PackageInfo[])ListViewWindowsApps.ItemsSource;
            var packagesToRemove = packages.Count(x => x.CheckedForRemoval);

            RunBackgroundProcess(packagesToRemove, RemoveCheckedAppWorker);
        }

        private void RemoveCheckedAppWorker(object sender, EventArgs e)
        {
            try
            {
                var packages = (PackageInfo[])ListViewWindowsApps.ItemsSource;
                var packagesToRemove = packages.Where(x => x.CheckedForRemoval);

                foreach (var ptr in packagesToRemove)
                {
                    _packageManager.RemovePackage(ptr.PackageName);
                    ptr.IsInstalled = false;
                    ptr.CheckedForRemoval = false;
                    LogWriter.LogInfo($"Removing Windows App: ${ptr.PackageName}");
                    AdvanceProgressBar();
                }

                ShowInfoMessage("Selected Windows Apps have been removed.");
            }
            catch (Exception ex)
            {
                LogWriter.LogError(ex);
                ShowErrorMessage(ex.Message);
            }
        }
    }
}
