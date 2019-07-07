using System;
using System.Windows;
using WereDev.Utils.Wu10Man.Helpers;

namespace WereDev.Utils.Wu10Man
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //ShowAdvancedItem.IsChecked = Properties.Settings.Default.ShowAdvanced;
            //SetMainScreenView(Properties.Settings.Default.ShowAdvanced);
            SetMainScreenView(true);
        }

        protected override void OnClosed(EventArgs e)
        {
            Application.Current.Shutdown();
            base.OnClosed(e);
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AboutItem_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new About();
            aboutWindow.Left = this.Left + ((this.Width - aboutWindow.Width) / 2);
            aboutWindow.Top = this.Top + ((this.Height - aboutWindow.Height) / 2);
            aboutWindow.ShowDialog();
        }

        private void LogFilesItem_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Wu10Logger.LogFolder);
        }

        private void ShowAdvancedItem_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ShowAdvanced = !Properties.Settings.Default.ShowAdvanced;
            Properties.Settings.Default.Save();
            SetMainScreenView(Properties.Settings.Default.ShowAdvanced);
        }

        private void SetMainScreenView(bool showAdvanced)
        {
            this.AdvancedControl.Visibility = showAdvanced ? Visibility.Visible : Visibility.Hidden;
            this.BasicOptions.Visibility = showAdvanced ? Visibility.Hidden : Visibility.Visible;
        }
    }
}

