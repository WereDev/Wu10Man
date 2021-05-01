using System;
using System.Collections.Generic;
using System.Linq;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;
using WereDev.Utils.Wu10Man.Core.Models;

namespace WereDev.Utils.Wu10Man.Core.Services
{
    public class HostsFileEditor : IHostsFileEditor
    {
        private const string HostsFilePath = @"drivers\etc\hosts";
        private const string Wu10ManStart = "# Start of entries added by Wu10Man";
        private const string OldWu10ManStart = "# Start of entried added by Wu10Man"; // was misspelled but left here for backwards compatibility
        private const string Wu10ManEnd = "# End of entries added by Wu10Man";
        private const string OldWu10ManEnd = "# End of entried added by Wu10Man"; // was misspelled but left here for backwards compatibility
        private const string NullEndpoint = "0.0.0.0\t";

        private readonly IFileIoProvider _fileIoProvider;
        private readonly string[] _windowsUpdateUrls;

        public HostsFileEditor(IFileIoProvider fileIoProvider, string[] windowsUpateUrls)
        {
            _fileIoProvider = fileIoProvider ?? throw new ArgumentNullException(nameof(fileIoProvider));
            _windowsUpdateUrls = windowsUpateUrls ?? new string[0];
        }

        private string HostsFile => _fileIoProvider.Combine(Environment.SystemDirectory, HostsFilePath);

        public void SetHostsEntries(IEnumerable<string> hostUrls)
        {
            var lines = ReadHostsFile();
            var split = SplitLines(lines);
            split.Wu10ManLines.Clear();
            var hosts = CreateLinesFromHosts(hostUrls);
            split.Wu10ManLines.AddRange(hosts);
            WriteHostsFile(split);
        }

        public void ClearHostsEntries()
        {
            var lines = ReadHostsFile();
            var split = SplitLines(lines);
            split.Wu10ManLines.Clear();
            WriteHostsFile(split);
        }

        public string[] GetHostsInFile()
        {
            var lines = ReadHostsFile();
            var split = SplitLines(lines);
            var hosts = GetHostsFromLines(split.Wu10ManLines);
            if (hosts == null)
                return Array.Empty<string>();
            else
                return hosts.Select(x => StandardizeHostUrl(x)).Distinct().ToArray();
        }

        public string[] GetManagedHosts()
        {
            var uniques = _windowsUpdateUrls.Select(x => StandardizeHostUrl(x)).Distinct();
            return uniques.ToArray();
        }

        public string[] GetLockingProcessNames()
        {
            var processes = _fileIoProvider.FindLockingProcesses(HostsFile);
            return processes.Select(x => x.MainModule?.FileVersionInfo?.ProductName ?? x.ProcessName).ToArray();
        }

        public void BlockHostUrl(string hostUrl)
        {
            if (string.IsNullOrWhiteSpace(hostUrl))
                throw new ArgumentNullException(nameof(hostUrl));
            hostUrl = StandardizeHostUrl(hostUrl);
            if (!GetManagedHosts().Contains(hostUrl))
                throw new InvalidOperationException("Host URL not monitored by Wu10Man: " + hostUrl);
            var currentHosts = GetHostsInFile();
            if (!currentHosts.Contains(hostUrl))
            {
                var hostsList = currentHosts.ToList();
                hostsList.Add(hostUrl);
                SetHostsEntries(hostsList);
            }
        }

        public void UnblockHostUrl(string hostUrl)
        {
            if (string.IsNullOrWhiteSpace(hostUrl))
                throw new ArgumentNullException(nameof(hostUrl));
            hostUrl = StandardizeHostUrl(hostUrl);
            if (!GetManagedHosts().Contains(hostUrl))
                throw new InvalidOperationException("Host URL not monitored by Wu10Man: " + hostUrl);
            var currentHosts = GetHostsInFile();
            if (currentHosts.Contains(hostUrl))
            {
                var hostsList = currentHosts.ToList();
                hostsList.Remove(hostUrl);
                SetHostsEntries(hostsList);
            }
        }

        private string StandardizeHostUrl(string hostUrl)
        {
            if (string.IsNullOrWhiteSpace(hostUrl))
                hostUrl = string.Empty;

            return hostUrl.Trim().ToLower();
        }

        private string[] GetHostsFromLines(IEnumerable<string> lines)
        {
            var hosts = lines.Select(x => GetHostFromLine(x))
                             .Where(x => !string.IsNullOrEmpty(x))
                             .ToArray();
            return hosts;
        }

        private string GetHostFromLine(string line)
        {
            line = line.Trim();
            if (string.IsNullOrEmpty(line))
                return line;
            var split = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            return split[1];
        }

        private string[] CreateLinesFromHosts(IEnumerable<string> hosts)
        {
            if (hosts?.Any() == true)
            {
                var lines = hosts.Select(x => NullEndpoint + x);
                return lines.ToArray();
            }

            return new string[0];
        }

        private string[] ReadHostsFile()
        {
            var lines = _fileIoProvider.ReadAllLines(HostsFile);
            return lines;
        }

        private SplitHostsFile SplitLines(IEnumerable<string> lines)
        {
            var splitLines = new SplitHostsFile();
            bool wu10Line = false;

            foreach (var line in lines)
            {
                switch (line)
                {
                    case Wu10ManStart:
                    case OldWu10ManStart:
                        wu10Line = true;
                        break;
                    case Wu10ManEnd:
                    case OldWu10ManEnd:
                        wu10Line = false;
                        break;
                    default:
                        if (wu10Line)
                            splitLines.Wu10ManLines.Add(line);
                        else
                            splitLines.OtherLines.Add(line);
                        break;
                }
            }

            return splitLines;
        }

        private void WriteHostsFile(SplitHostsFile splitHostsFile)
        {
            List<string> lines = new List<string>(splitHostsFile.OtherLines);

            if (splitHostsFile.Wu10ManLines.Any())
            {
                lines.Add(Wu10ManStart);
                lines.AddRange(splitHostsFile.Wu10ManLines);
                lines.Add(Wu10ManEnd);
            }

            _fileIoProvider.WriteAllLines(HostsFile, lines);
        }
    }
}
