using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WereDev.Utils.Wu10Man
{
    public static class DependencyManager
    {
        public static IContainer Container { get; set; }

        public static T Resolve<T>() => Container.Resolve<T>();
    }
}
