using System;
using WereDev.Utils.Win32Wrappers;
using WereDev.Utils.Wu10Man.Core.Enums;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;

namespace WereDev.Utils.Wu10Man.Providers
{
    public class WindowsApiProvider : IWindowsApiProvider
    {
        public void ModifyPrivilege(WindowsApiPrivelegeNames privilege, bool enable)
        {
            var privlegeName = (PrivilegeName)Enum.Parse(typeof(PrivilegeName), privilege.ToString());
            WindowsApiBridge.ModifyPrivilege(privlegeName, enable);
        }
    }
}
