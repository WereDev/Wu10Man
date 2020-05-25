using WereDev.Utils.Wu10Man.Core.Models;

namespace WereDev.Utils.Wu10Man.Core.Interfaces.Providers
{
    public interface IConfigurationReader
    {
        string[] GetWindowsUpdateHosts();

        string[] GetWindowsServices();

        Declutter GetDeclutter();
    }
}
