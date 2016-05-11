using Microsoft.Practices.Unity;

using NuClear.Messaging.Transports.CorporateBus;
using NuClear.Messaging.Transports.CorporateBus.API;
using NuClear.Messaging.Transports.CorporateBus.Flows;

namespace NuClear.CustomerIntelligence.Replication.Host.Factories
{
    public class UnityCorporateBusMessageFlowReceiverFactory : ICorporateBusMessageFlowReceiverFactory
    {
        private readonly IUnityContainer _container;

        public UnityCorporateBusMessageFlowReceiverFactory(IUnityContainer container)
        {
            _container = container;
        }

        public ICorporateBusMessageFlowReceiver Create(ICorporateBusFlow messageFlow)
        {
            return _container.Resolve<CorporateBusMessageFlowReceiver>(new DependencyOverride(typeof(ICorporateBusFlow), messageFlow));
        }
    }
}