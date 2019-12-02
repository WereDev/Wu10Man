namespace WereDev.Utils.Wu10Man.Core.Interfaces.Providers
{
    public interface IWindowsServiceProviderFactory
    {
        IWindowsServiceProvider GetWindowsServiceProvider(string service);
    }
}
