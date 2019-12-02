using System.Security.Principal;

namespace WereDev.Utils.Wu10Man.Core.Interfaces.Providers
{
    public interface IUserProvider
    {
        SecurityIdentifier GetCurrentUser();
    }
}
