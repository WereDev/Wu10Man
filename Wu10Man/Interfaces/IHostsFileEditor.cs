using System.Collections.Generic;

namespace WereDev.Utils.Wu10Man.Interfaces
{
    public interface IHostsFileEditor
    {
        void SetHostsEntries(IEnumerable<string> hostUrls);

        void ClearHostsEntries();

        string[] GetHostsInFile();

        string[] GetLockingProcessNames();
    }
}
