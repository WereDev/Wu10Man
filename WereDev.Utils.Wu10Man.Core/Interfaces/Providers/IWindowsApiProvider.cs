using WereDev.Utils.Wu10Man.Core.Enums;

namespace WereDev.Utils.Wu10Man.Core.Interfaces.Providers
{
    public interface IWindowsApiProvider
    {
        void ModifyPrivilege(WindowsApiPrivelegeNames privilege, bool enable);
    }
}
