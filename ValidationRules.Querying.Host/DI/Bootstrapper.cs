using Microsoft.Practices.Unity;

using NuClear.ValidationRules.Querying.Host.DataAccess;

namespace NuClear.ValidationRules.Querying.Host.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity()
        {
            return new UnityContainer()
                .ConfigureDataAccess();
        }

        private static IUnityContainer ConfigureDataAccess(this IUnityContainer container)
        {
            return container
                .RegisterType<DataConnectionFactory>(new ContainerControlledLifetimeManager())
                .RegisterType<MessageRepositiory>(new PerResolveLifetimeManager());
        }
    }
}