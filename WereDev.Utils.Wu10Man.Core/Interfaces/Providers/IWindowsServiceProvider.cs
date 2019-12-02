using System;
using System.ServiceProcess;

namespace WereDev.Utils.Wu10Man.Core.Interfaces.Providers
{
    public interface IWindowsServiceProvider : IDisposable
    {
        string DisplayName { get; }

        bool SetStartupType(ServiceStartMode startMode);

        bool ServiceExists();

        bool TryDisableService();

        bool TryEnableService();

        bool IsServiceEnabled();

        void StopService();

        void SetAccountAsLocalService();

        void SetAccountAsLocalSystem();

        bool IsServiceRunAsLocalSystem();
    }
}
