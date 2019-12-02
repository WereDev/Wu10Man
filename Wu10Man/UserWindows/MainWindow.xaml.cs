using System;
using System.Windows;
using WereDev.Utils.Wu10Man.Core;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.Services;

namespace WereDev.Utils.Wu10Man.UserWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILogWriter _logWriter;

        public MainWindow()
        {
            _logWriter = DependencyManager.Resolve<ILogWriter>();

            _logWriter.LogInfo("Main window initializing.");
            InitializeComponent();
            _logWriter.LogInfo("Main window initialized.");
        }

        protected override void OnClosed(EventArgs e)
        {
            Application.Current.Shutdown();
            base.OnClosed(e);
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AboutItem_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new About();
            aboutWindow.Left = Left + ((Width - aboutWindow.Width) / 2);
            aboutWindow.Top = Top + ((Height - aboutWindow.Height) / 2);
            aboutWindow.ShowDialog();
        }

        private void LogFilesItem_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start((_logWriter as Wu10Logger)?.LogFolder);
        }

        private void ReadmeItem_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/WereDev/Wu10Man/blob/master/README.md");
        }
    }
}
