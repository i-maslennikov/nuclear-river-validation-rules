using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Config;

namespace NuClear.CustomerIntelligence.Replication.Host.DI
{
    internal static class UnityContainerExtensions
    {
        public static IUnityContainer RegisterOne2ManyTypesPerTypeUniqueness<TFrom, TTo>(this IUnityContainer container, LifetimeManager lifetimeManager, params InjectionMember[] members)
        {
            container.RegisterTypeWithDependencies(
                typeof(TFrom),
                typeof(TTo),
                typeof(TTo).GetPerTypeUniqueMarker(),
                lifetimeManager,
                (string)null,
                members);

            return container;
        }
    }
}