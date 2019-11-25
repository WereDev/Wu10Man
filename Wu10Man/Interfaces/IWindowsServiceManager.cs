namespace WereDev.Utils.Wu10Man.Interfaces
{
    public interface IWindowsServiceManager
    {
        string[] ListAllServices();

        bool ServiceExists(string serviceName);

        string GetServiceDisplayName(string serviceName);

        string GetServiceDllPath(string serviceName);

        bool AreAllServicesEnabled();

        bool AreAllServicesDisabled();

        bool IsServiceEnabled(string serviceName);

        bool EnableService(string serviceName);

        void DisableService(string serviceName);

        void AddWu10ToFileName(string serviceName);

        void RemoveWu10FromFileName(string serviceName);
    }
}
