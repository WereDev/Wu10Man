namespace WereDev.Utils.Wu10Man.Core.Models
{
    public class PackageInfo
    {
        public string PackageName { get; set; }

        public string InstallLocation { get; set; }

        public override string ToString()
        {
            return PackageName;
        }
    }
}
