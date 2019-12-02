using System.Collections.Generic;

namespace WereDev.Utils.Wu10Man.Core.Models
{
    internal class SplitHostsFile
    {
        public SplitHostsFile()
        {
            Wu10ManLines = new List<string>();
            OtherLines = new List<string>();
        }

        public List<string> Wu10ManLines { get; }

        public List<string> OtherLines { get; }
    }
}
