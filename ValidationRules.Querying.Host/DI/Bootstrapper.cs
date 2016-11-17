using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.Querying.Host.DataAccess;

namespace NuClear.ValidationRules.Querying.Host.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity()
        {
            return new UnityContainer()
                .ConfigureDataAccess()
                .ConfigureSerializers();
        }

        private static IUnityContainer ConfigureDataAccess(this IUnityContainer container)
        {
            return container
                .RegisterType<DataConnectionFactory>(new ContainerControlledLifetimeManager())
                .RegisterType<MessageRepositiory>(new PerResolveLifetimeManager());
        }

        private static IUnityContainer ConfigureSerializers(this IUnityContainer container)
        {
            var interfaceType = typeof(IMessageComposer);
            var serializerTypes = interfaceType.Assembly.GetTypes()
                                               .Where(x => interfaceType.IsAssignableFrom(x) && x.IsClass && !x.IsAbstract)
                                               .ToArray();

            container.RegisterType(typeof(IReadOnlyCollection<IMessageComposer>),
                                   new InjectionFactory(c => serializerTypes.Select(t => c.Resolve(t)).Cast<IMessageComposer>().ToArray()));

            return container;
        }
    }
}