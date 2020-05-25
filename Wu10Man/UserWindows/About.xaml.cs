using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace WereDev.Utils.Wu10Man.UserWindows
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            var version = GetType().Assembly.GetName().Version;
            lblAppVersion.Text = "v" + version.ToString();
        }

        private void GitHub_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Hyperlink linkAnnouncements = (Hyperlink)sender;
            string site = linkAnnouncements.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(site));
            e.Handled = true;
        }
    }
}
