using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using WereDev.Utils.Wu10Man.Core.Exceptions;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;

namespace WereDev.Utils.Wu10Man.Core.Services
{
    public class FileManager : IFileManager
    {
        private readonly string _trustedInstallerUser = @"NT SERVICE\TrustedInstaller";
        private readonly IWindowsApiProvider _windowsApiProvider;
        private readonly IFileIoProvider _fileIoProvider;
        private readonly ICredentialsProvider _credentialsProvider;

        public FileManager(IFileIoProvider fileIoProvider, IWindowsApiProvider windowsApiProvider, ICredentialsProvider credentialsProvider)
        {
            _fileIoProvider = fileIoProvider ?? throw new ArgumentNullException(nameof(fileIoProvider));
            _windowsApiProvider = windowsApiProvider ?? throw new ArgumentNullException(nameof(windowsApiProvider));
            _credentialsProvider = credentialsProvider ?? throw new ArgumentNullException(nameof(credentialsProvider));
        }

        public void RenameFile(string origPath, string newPath)
        {
            if (_fileIoProvider.Exists(origPath))
            {
                _fileIoProvider.Move(origPath, newPath);
            }
        }

        public void TakeOwnership(string fileName, string userName)
        {
            SetOwnership(fileName, userName);
        }

        public void GiveOwnershipToAdministrators(string fileName)
        {
            var username = _credentialsProvider.GetAccountAdministratorUserName();
            SetOwnership(fileName, username);
            GrantFullAccessToFile(fileName, username);
        }

        public void GiveOwnershipToTrustedInstaller(string fileName)
        {
            SetOwnership(fileName, _trustedInstallerUser);
        }

        public void SetOwnership(string fileName, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException(nameof(userName));

            var fileSecurity = GetFileSecurity(fileName);
            var account = new NTAccount(userName);
            fileSecurity.SetOwner(account);
            _fileIoProvider.SetAccessControl(fileName, fileSecurity);
        }

        public void GrantFullAccessToFile(string fileName, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException(nameof(userName));

            var fileSecurity = GetFileSecurity(fileName);
            var account = new NTAccount(userName);
            fileSecurity.SetAccessRule(new FileSystemAccessRule(account, FileSystemRights.FullControl, AccessControlType.Allow));
            _fileIoProvider.SetAccessControl(fileName, fileSecurity);
        }

        public string[] ReadAllLines(string path)
        {
            return _fileIoProvider.ReadAllLines(path);
        }

        public void WriteAllLines(string path, IEnumerable<string> lines)
        {
            _fileIoProvider.WriteAllLines(path, lines);
        }

        public string[] FindLockingProcesses(string path)
        {
            var processes = _fileIoProvider.FindLockingProcesses(path);
            return processes.Select(x => x.MainModule?.FileVersionInfo?.ProductName ?? x.ProcessName).ToArray();
        }

        public string GetFileName(string path)
        {
            return _fileIoProvider.GetFileName(path);
        }

        public string Combine(string path1, string path2)
        {
            return _fileIoProvider.Combine(path1, path2);
        }

        public bool Exists(string path)
        {
            return _fileIoProvider.Exists(path);
        }

        public void Delete(string path)
        {
            _fileIoProvider.Delete(path);
        }

        public string GetDirectoryName(string path)
        {
            return _fileIoProvider.GetDirectoryName(path);
        }

        private FileSecurity GetFileSecurity(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (!_fileIoProvider.Exists(fileName))
                throw new EntityNotFoundException("Could not find file to set ownership.", fileName);

            // Allow this process to circumvent ACL restrictions
            _windowsApiProvider.ModifyPrivilege(Enums.WindowsApiPrivelegeNames.SeRestorePrivilege, true);

            // Sometimes this is required and other times it works without it. Not sure when.
            _windowsApiProvider.ModifyPrivilege(Enums.WindowsApiPrivelegeNames.SeTakeOwnershipPrivilege, true);

            var accessControl = _fileIoProvider.GetAccessControl(fileName);

            return accessControl;
        }
    }
}
