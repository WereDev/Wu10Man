using System;
using System.Collections.Generic;
using System.Linq;
using WereDev.Utils.Wu10Man.Editors;

namespace WereDev.Utils.Wu10Man.Helpers
{
    internal class HostsFileHelper
    {
        private readonly HostsEditor _hostsEditor;
        private readonly HashSet<string> _hostUrls;
        private readonly Wu10Logger _logger;

        public HostsFileHelper()
        {
            _hostsEditor = new HostsEditor();
            _hostUrls = GetWindowsUpdateUrls();
            _logger = new Wu10Logger();
        }

        //TODO: I don't like this here because of the dependency on ConfigurationManager.
        private HashSet<string> GetWindowsUpdateUrls()
        {
            var windowsUpdateUrls = System.Configuration.ConfigurationManager.AppSettings["WindowsUpdateUrls"];
            if (string.IsNullOrWhiteSpace(windowsUpdateUrls)) return new HashSet<string>();
            windowsUpdateUrls = StandardizeHostUrl(windowsUpdateUrls);
            var split = windowsUpdateUrls.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var uniques = split.Select(x => StandardizeHostUrl(x)).Distinct();
            return new HashSet<string>(uniques);
        }

        private string StandardizeHostUrl(string hostUrl)
        {
            if (String.IsNullOrWhiteSpace(hostUrl)) hostUrl = string.Empty;
            return hostUrl.Trim().ToLower();
        }

        public void BlockHostUrl(string hostUrl)
        {
            if (string.IsNullOrWhiteSpace(hostUrl))
                throw new ArgumentNullException(nameof(hostUrl));
            hostUrl = StandardizeHostUrl(hostUrl);
            if (!_hostUrls.Contains(hostUrl))
                throw new InvalidOperationException("Host URL not monitored by Wu10Man: " + hostUrl);
            var currentHosts = GetBlockedHostUrls();
            if (!currentHosts.Contains(hostUrl))
            {
                var hostsList = currentHosts.ToList();
                hostsList.Add(hostUrl);
                _hostsEditor.SetHostsEntries(hostsList);
                _logger.LogInfo(string.Format("Host blocked: {0}", hostUrl));
            }
        }

        public void BlockAllHostUrls()
        {
            _hostsEditor.SetHostsEntries(_hostUrls);
            _logger.LogInfo("All Hosts blocked.");

        }

        public void UnblockHostUrl(string hostUrl)
        {
            if (string.IsNullOrWhiteSpace(hostUrl))
                throw new ArgumentNullException(nameof(hostUrl));
            hostUrl = StandardizeHostUrl(hostUrl);
            if (!_hostUrls.Contains(hostUrl))
                throw new InvalidOperationException("Host URL not monitored by Wu10Man: " + hostUrl);
            var currentHosts = GetBlockedHostUrls();
            if (currentHosts.Contains(hostUrl))
            {
                var hostsList = currentHosts.ToList();
                hostsList.Remove(hostUrl);
                _hostsEditor.SetHostsEntries(hostsList);
                _logger.LogInfo(string.Format("Host unblocked: {0}", hostUrl));
            }
        }

        public void UnblockAllHostUrls()
        {
            _hostsEditor.ClearHostsEntries();
            _logger.LogInfo("All Hosts unblocked.");
        }

        public string[] GetBlockedHostUrls()
        {
            var currentHosts = _hostsEditor.GetHostsInFile();
            if (currentHosts == null) return new string[0];
            return currentHosts.Select(x => StandardizeHostUrl(x)).Distinct().ToArray();
        }

        public string[] GetManagedHostUrls()
        {
            return _hostUrls.ToArray();
        }
    }
}
