using System;
using System.ServiceProcess;

namespace WereDev.Utils.Wu10Man.Interfaces
{
    public interface IWindowsServiceProvider : IDisposable
    {
        string DisplayName { get; }

        bool SetStartupType(ServiceStartMode startMode);

        bool ServiceExists();

        string GetServiceDLL();

        void DisableService();

        bool EnableService();

        bool IsServiceEnabled();

        bool IsServiceRunAsLocalSystem();

        void StopService();

        void SetAccountAsLocalService();

        void SetAccountAsLocalSystem();
    }
}
