using System.Security.Principal;

namespace WereDev.Utils.Wu10Man.Interfaces
{
    public interface IServiceCredentialsEditor
    {
        string GetUserName(WellKnownSidType sidType);

        string GetWindowsServiceUserName(string serviceName);

        void SetWindowsServiceCredentials(string serviceName, WellKnownSidType sidType);

        void SetWindowsServiceCredentials(string serviceName, string username, string password);
    }
}
