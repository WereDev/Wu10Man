using System.Security.Principal;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;

namespace WereDev.Utils.Wu10Man.Providers
{
    public class UserProvider : IUserProvider
    {
        public SecurityIdentifier GetCurrentUser()
        {
            return WindowsIdentity.GetCurrent().User;
        }
    }
}
