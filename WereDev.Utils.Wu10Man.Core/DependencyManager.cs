using Autofac;

namespace WereDev.Utils.Wu10Man.Core
{
    public static class DependencyManager
    {
        public static IContainer Container { get; set; }

        public static T Resolve<T>() => Container.Resolve<T>();
    }
}
