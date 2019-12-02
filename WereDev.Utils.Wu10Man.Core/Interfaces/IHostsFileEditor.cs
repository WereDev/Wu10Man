using System.Collections.Generic;

namespace WereDev.Utils.Wu10Man.Core.Interfaces
{
    public interface IHostsFileEditor
    {
        void SetHostsEntries(IEnumerable<string> hostUrls);

        void ClearHostsEntries();

        string[] GetHostsInFile();

        string[] GetLockingProcessNames();

        void BlockHostUrl(string hostUrl);

        void UnblockHostUrl(string hostUrl);

        string[] GetManagedHosts();
    }
}
