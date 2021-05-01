using WereDev.Utils.Wu10Man.Core.Models;

namespace WereDev.Utils.Wu10Man
{
    internal class AppSettings
    {
        public string[] WindowsUpdateUrls { get; set; }

        public string[] WindowsServices { get; set; }

        public DeclutterConfig Declutter { get; set; }

        public WindowsTaskConfig[] WindowsTasks { get; set; }
    }
}
