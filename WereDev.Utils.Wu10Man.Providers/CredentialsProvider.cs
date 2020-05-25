using System.Security.Principal;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;

namespace WereDev.Utils.Wu10Man.Providers
{
    public class CredentialsProvider : ICredentialsProvider
    {
        public string GetAccountAdministratorUserName()
        {
            return GetUserName(WellKnownSidType.AccountAdministratorSid);
        }

        public string GetUserName(WellKnownSidType sidType)
        {
            var sid = new SecurityIdentifier(sidType, WindowsIdentity.GetCurrent().User.AccountDomainSid);
            var account = sid.Translate(typeof(NTAccount));
            return account.Value;
        }
    }
}
