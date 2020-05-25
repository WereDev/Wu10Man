namespace WereDev.Utils.Wu10Man.Core.Models
{
    public class AppInfoExtended : AppInfo
    {
        public AppInfoExtended(AppInfo appInfo)
        {
            AppName = appInfo?.AppName;
            PackageName = appInfo?.PackageName;
        }

        public bool IsInstalled { get; set; }

        public string IconPath { get; set; }

        public override string ToString()
        {
            return AppName;
        }
    }
}
