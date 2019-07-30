using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WereDev.Utils.Wu10Man
{
    public static class Constants
    {
        public const string USERNAME_LOCAL_SYSTEM = @".\LocalSystem";

        public const string SERVICE_WINDOWS_UPDATE = "wuauserv";
        public const string SERVICE_MODULES_INSTALLER = "TrustedInstaller";
        public const string SERVICE_UPDATE_MEDIC = "WaaSMedicSvc";
        public const string SERVICE_SHOULD_NOT_EXIST = "ShouldNotExist";
    }
}
