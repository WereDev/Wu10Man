using System;
using System.Linq;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;

namespace WereDev.Utils.Wu10Man.Helpers
{
    internal class ConfigurationReader : IConfigurationReader
    {
        public string[] GetWindowsServices()
        {
            var array = GetStringArray("WindowsServiceNames");
            array = array.Select(x => x.Trim()).ToArray();
            return array;
        }

        public string[] GetWindowsUpdateHosts()
        {
            var array = GetStringArray("WindowsUpdateUrls");
            array = array.Select(x => x.ToLower().Trim()).ToArray();
            return array;
        }

        private string[] GetStringArray(string key)
        {
            var appSetting = System.Configuration.ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(appSetting))
                return new string[0];
            var split = appSetting.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var uniques = split.Distinct().ToArray();
            return uniques;
        }
    }
}
