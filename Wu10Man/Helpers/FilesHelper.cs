using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using WereDev.Utils.Win32Wrappers;
using WereDev.Utils.Wu10Man.Interfaces;

namespace WereDev.Utils.Wu10Man.Helpers
{
    public class FilesHelper : IFilesHelper
    {
        private readonly string _trustedInstallerUser = @"NT SERVICE\TrustedInstaller";
        private readonly IServiceCredentialsEditor _serviceCredentialsEditor;

        public FilesHelper(IServiceCredentialsEditor serviceCredentialsEditor)
        {
            _serviceCredentialsEditor = serviceCredentialsEditor ?? throw new ArgumentNullException(nameof(serviceCredentialsEditor));
        }

        public void RenameFile(string origPath, string newPath)
        {
            if (File.Exists(origPath))
            {
                File.Move(origPath, newPath);
            }
        }

        public void TakeOwnership(string fileName, string userName)
        {
            SetOwnership(fileName, userName);
        }

        public void GiveOwnershipToAdministrators(string fileName)
        {
            var username = _serviceCredentialsEditor.GetUserName(WellKnownSidType.AccountAdministratorSid);
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
            File.SetAccessControl(fileName, fileSecurity);
        }

        public void GrantFullAccessToFile(string fileName, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException(nameof(userName));

            var fileSecurity = GetFileSecurity(fileName);
            var account = new NTAccount(userName);
            fileSecurity.SetAccessRule(new FileSystemAccessRule(account, FileSystemRights.FullControl, AccessControlType.Allow));
            File.SetAccessControl(fileName, fileSecurity);
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        private FileSecurity GetFileSecurity(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (!File.Exists(fileName))
                throw new FileNotFoundException("Could not find file to set ownership.", fileName);

            // Allow this process to circumvent ACL restrictions
            WinApiWrapper.ModifyPrivilege(PrivilegeName.SeRestorePrivilege, true);

            // Sometimes this is required and other times it works without it. Not sure when.
            WinApiWrapper.ModifyPrivilege(PrivilegeName.SeTakeOwnershipPrivilege, true);

            var accessControl = File.GetAccessControl(fileName);

            return accessControl;
        }
    }
}
