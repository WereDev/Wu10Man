using System;
using System.Windows;

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
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AboutItem_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new About
            {
                Owner = this
            };
            aboutWindow.Left = this.Left + ((this.Width - aboutWindow.Width) / 2);
            aboutWindow.Top = this.Top + ((this.Height - aboutWindow.Height) / 2);
            aboutWindow.ShowDialog();
        }
    }
}

