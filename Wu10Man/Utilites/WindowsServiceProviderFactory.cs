using WereDev.Utils.Wu10Man.Interfaces;

namespace WereDev.Utils.Wu10Man.Utilites
{
    internal class WindowsServiceProviderFactory : IWindowsServiceProviderFactory
    {
        public IWindowsServiceProvider GetWindowsServiceProvider(string service)
        {
            var registryEditor = DependencyManager.Resolve<IRegistryEditor>();
            var serviceCredentialsEditor = DependencyManager.Resolve<IServiceCredentialsEditor>();
            return new WindowsServiceProvider(service, registryEditor, serviceCredentialsEditor);
        }
    }
}
