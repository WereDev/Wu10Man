using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Controls;
using WereDev.Utils.Wu10Man.Editors;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for HostsFileControl.xaml
    /// </summary>
    public partial class HostsFileControl : UserControl
    {
        private readonly HostsEditor _hostsEditor = new HostsEditor();

        public Dictionary<string, bool> HostSettings { get; private set; }


        public HostsFileControl()
        {
            SetHostSettings();
            InitializeComponent();
        }

        public void ResetHostSettings()
        {
            SetHostSettings();
        }

        public void SaveHostSettings()
        {
            var selectedHosts = HostSettings.Where(x => x.Value).Select(x => x.Key);
            if (selectedHosts.Any())
                _hostsEditor.SetHostsEntries(selectedHosts);
            else
                _hostsEditor.ClearHostsEntries();

        }

        private void SetHostSettings()
        {
            var hostUrls = GetWindowsUpdateUrls();
            var currentHosts = _hostsEditor.GetHostsInFile();
            var hostSettings = hostUrls.ToDictionary(x => x, x => currentHosts.Contains(x));
            HostSettings = hostSettings;
        }

        private string[] GetWindowsUpdateUrls()
        {
            var windowsUpdateUrls = ConfigurationManager.AppSettings["WindowsUpdateUrls"];
            var split = windowsUpdateUrls.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var uniques = split.Distinct();
            return uniques?.ToArray();
        }
    }
}
