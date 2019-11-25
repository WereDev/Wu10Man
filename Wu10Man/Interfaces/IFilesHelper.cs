namespace WereDev.Utils.Wu10Man.Interfaces
{
    interface IFilesHelper
    {
        void RenameFile(string origPath, string newPath);

        void TakeOwnership(string fileName, string userName);

        void GiveOwnershipToAdministrators(string fileName);

        void GiveOwnershipToTrustedInstaller(string fileName);

        void SetOwnership(string fileName, string userName);

        void GrantFullAccessToFile(string fileName, string userName);

        string GetDirectoryName(string path);

        string GetFileName(string path);

        string Combine(string path1, string path2);

        bool Exists(string path);

        void Delete(string path);
    }
}
