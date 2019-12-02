using System;
using WereDev.Utils.Win32Wrappers;
using WereDev.Utils.Wu10Man.Core.Enums;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;

namespace WereDev.Utils.Wu10Man.Providers
{
    public class WindowsApiAdapter : IWindowsApiProvider
    {
        public void ModifyPrivilege(WindowsApiPrivelegeNames privilege, bool enable)
        {
            var privelegeName = (PrivilegeName)Enum.Parse(typeof(PrivilegeName), privilege.ToString());
            WindowsApiBridge.ModifyPrivilege(privelegeName, enable);
        }
    }
}
