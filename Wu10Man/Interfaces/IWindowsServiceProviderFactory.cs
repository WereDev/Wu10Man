using System;

namespace WereDev.Utils.Wu10Man.Interfaces
{
    public interface IWindowsServiceProviderFactory
    {
        IWindowsServiceProvider GetWindowsServiceProvider(string service);
    }
}
