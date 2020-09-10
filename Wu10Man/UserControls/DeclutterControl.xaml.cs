using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WereDev.Utils.Wu10Man.Core;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.UserControls.Models;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for DeclutterControl.xaml.
    /// </summary>
    public partial class DeclutterControl : UserControl, IDisposable
    {
        private const string TabTitle = "Declutter";
        private readonly ILogWriter _logWriter;
        private readonly IWindowsPackageManager _packageManager;
        private readonly DeclutterModel _model;
        private BackgroundWorker _worker;
        private bool _isDisposed = false;

        public DeclutterControl()
        {
            _logWriter = DependencyManager.LogWriter;
            _packageManager = DependencyManager.WindowsPackageManager;
            _model = new DeclutterModel();

            _logWriter.LogInfo("Declutter Control initializing.");
        }

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                SetRuntimeOptions();
            }

            base.OnRender(drawingContext);

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                _worker.Dispose();
            }

            _isDisposed = true;
        }

        private bool SetRuntimeOptions()
        {
            try
            {
                GetPackageStatus();
                DataContext = _model;
                _logWriter.LogInfo("Declutter Control rendered.");
                return true;
            }
            catch (Exception ex)
            {
                _logWriter.LogError(ex);
                MessageBox.Show($"Error rendering {TabTitle} tab.", TabTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                InitializeComponent();
            }
        }

        private void GetPackageStatus()
        {
            var declutter = _packageManager.GetDeclutterConfig();
            var installedApps = _packageManager.ListInstalledPackages();
            var microsoftApps = _packageManager.MergePackageInfo(declutter.Microsoft, installedApps);
            _model.SetPackages(DeclutterModel.PackageSources.Microsoft, microsoftApps.Select(x => new PackageInfo(x)));
            var thirdPartyApps = _packageManager.MergePackageInfo(declutter.ThirdParty, installedApps);
            _model.SetPackages(DeclutterModel.PackageSources.ThirdParty, thirdPartyApps.Select(x => new PackageInfo(x)));
        }

        private void ShowMicrosoftApps(object sender, RoutedEventArgs e)
        {
            _model.PackageSource = DeclutterModel.PackageSources.Microsoft;
        }

        private void ShowThirdPartyApps(object sender, RoutedEventArgs e)
        {
            _model.PackageSource = DeclutterModel.PackageSources.ThirdParty;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "_worker part of larger scope.")]
        private void RemoveCheckedApps(object sender, RoutedEventArgs e)
        {
            var packages = (PackageInfo[])ListViewWindowsApps.ItemsSource;
            var packagesToRemove = packages.Where(x => x.CheckedForRemoval).ToArray();
            InitializeProgressBar(0, packagesToRemove.Length);

            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
            };
            _worker.DoWork += RemoveCheckedAppWorker;
            _worker.RunWorkerCompleted += RunWorkerCompleted;
            _worker.RunWorkerAsync();
        }

        private void ToggleAppsSelection(object sender, RoutedEventArgs e)
        {
            var setChecked = !_model.AllPackagesSelected;

            foreach (var package in _model.Packages)
            {
                if (package.CheckedForRemoval != setChecked)
                {
                    package.CheckedForRemoval = setChecked;
                }
            }
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ShowMessage("Selected Windows Apps have been removed.");
            ProgressBar.Visibility = Visibility.Hidden;
        }

        private void RemoveCheckedAppWorker(object sender, EventArgs e)
        {
            var packages = (PackageInfo[])ListViewWindowsApps.ItemsSource;
            var packagesToRemove = packages.Where(x => x.CheckedForRemoval).ToArray();

            foreach (var ptr in packagesToRemove)
            {
                _packageManager.RemovePackage(ptr.PackageName);
                ptr.IsInstalled = false;
                ptr.CheckedForRemoval = false;
                _logWriter.LogInfo($"Removing Windows App: ${ptr.PackageName}");
                AdvanceProgressBar();
            }
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(message, TabTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void InitializeProgressBar(int minValue, int maxValue)
        {
            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.Initialize(minValue, maxValue);
        }

        private void AdvanceProgressBar()
        {
            ProgressBar.Advance();
        }
    }
}
