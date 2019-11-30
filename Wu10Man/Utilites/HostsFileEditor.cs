using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WereDev.Utils.Win32Wrappers;
using WereDev.Utils.Wu10Man.Editors.Models;
using WereDev.Utils.Wu10Man.Interfaces;

namespace WereDev.Utils.Wu10Man.Utilites
{
    internal class HostsFileEditor : IHostsFileEditor
    {
        private const string HostsFilePath = @"drivers\etc\hosts";
        private const string Wu10ManStart = "# Start of entries added by Wu10Man";
        private const string Wu10ManEnd = "# End of entries added by Wu10Man";
        private const string NullEndpoint = "0.0.0.0\t";

        private string HostsFile => Path.Combine(Environment.SystemDirectory, HostsFilePath);

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
            return hosts;
        }

        public string[] GetLockingProcessNames()
        {
            var processes = FileWrapper.WhoIsLocking(HostsFile);

            if (processes == null)
                return new string[0];

            return processes.Select(x => x.MainModule?.FileVersionInfo?.ProductName ?? x.ProcessName).ToArray();
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
            var lines = hosts.Select(x => NullEndpoint + x);
            return lines.ToArray();
        }

        private string[] ReadHostsFile()
        {
            var lines = File.ReadLines(HostsFile).ToArray();
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
                        wu10Line = true;
                        break;
                    case Wu10ManEnd:
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

            File.WriteAllLines(HostsFile, lines);
        }
    }
}
