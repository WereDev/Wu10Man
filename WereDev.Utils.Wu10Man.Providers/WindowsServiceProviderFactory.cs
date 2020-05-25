using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;

namespace WereDev.Utils.Wu10Man.Providers
{
    public class WindowsServiceProviderFactory : IWindowsServiceProviderFactory
    {
        public IWindowsServiceProvider GetWindowsServiceProvider(string service)
        {
            return new WindowsServiceProvider(service);
        }
    }
}
