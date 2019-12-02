using System.Collections.Generic;
using System.Diagnostics;
using System.Security.AccessControl;

namespace WereDev.Utils.Wu10Man.Core.Interfaces.Providers
{
    public interface IFileIoProvider
    {
        Process[] FindLockingProcesses(string path);

        bool Exists(string path);

        void Delete(string path);

        void Move(string sourceFile, string destFile);

        FileSecurity GetAccessControl(string path);

        void SetAccessControl(string path, FileSecurity fileSecurity);

        string GetFileName(string path);

        string GetDirectoryName(string path);

        string Combine(string path1, string path2);

        string[] ReadAllLines(string path);

        void WriteAllLines(string path, IEnumerable<string> lines);
    }
}
