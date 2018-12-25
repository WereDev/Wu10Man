using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using WereDev.Utils.Wu10Man.Editors;

namespace WereDev.Utils.Wu10Man.Helpers
{
    internal class FilesHelper
    {

        private readonly string _trustedInstallerUser = @"NT SERVICE\TrustedInstaller";

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
            var username = ServiceCredentialsEditor.GetUserName(WellKnownSidType.AccountAdministratorSid);
            SetOwnership(fileName, username);
            GrantFullAccessToFile(fileName, username);
        }

        public void GiveOwnershipToTrustedInstaller(string fileName)
        {
            SetOwnership(fileName, _trustedInstallerUser);
        }

        public void SetOwnership(string fileName, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));

            var fileSecurity = GetFileSecurity(fileName);
            var account = new NTAccount(userName);
            fileSecurity.SetOwner(account);
            File.SetAccessControl(fileName, fileSecurity);
        }

        public void GrantFullAccessToFile(string fileName, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));

            var fileSecurity = GetFileSecurity(fileName);
            var account = new NTAccount(userName);
            fileSecurity.SetAccessRule(new FileSystemAccessRule(account, FileSystemRights.FullControl, AccessControlType.Allow));
            File.SetAccessControl(fileName, fileSecurity);
        }

        private FileSecurity GetFileSecurity(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
            if (!File.Exists(fileName)) throw new FileNotFoundException("Could not find file to set ownership.", fileName);

            // Allow this process to circumvent ACL restrictions
            WinAPI.ModifyPrivilege(PrivilegeName.SeRestorePrivilege, true);

            // Sometimes this is required and other times it works without it. Not sure when.
            WinAPI.ModifyPrivilege(PrivilegeName.SeTakeOwnershipPrivilege, true);

            var accessControl = File.GetAccessControl(fileName);

            return accessControl;
        }
    }
}
