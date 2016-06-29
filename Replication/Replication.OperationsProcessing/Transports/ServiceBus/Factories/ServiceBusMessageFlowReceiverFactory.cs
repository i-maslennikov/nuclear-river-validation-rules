using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.ServiceBus;
using NuClear.Messaging.Transports.ServiceBus.API;
using NuClear.Tracing.API;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus.Factories
{
    public class ServiceBusMessageFlowReceiverFactory : IServiceBusMessageFlowReceiverFactory
    {
        private readonly ITracer _tracer;
        private readonly IServiceBusLockRenewer _renewer;
        private readonly IServiceBusSettingsFactory _settingsFactory;

        public ServiceBusMessageFlowReceiverFactory(ITracer tracer, IServiceBusLockRenewer renewer, IServiceBusSettingsFactory settingsFactory)
        {
            _tracer = tracer;
            _renewer = renewer;
            _settingsFactory = settingsFactory;
        }

        public IServiceBusMessageFlowReceiver Create(IMessageFlow messageFlow)
        {
            var settings = _settingsFactory.CreateReceiverSettings(messageFlow);
            return new ServiceBusMessageFlowReceiver(_tracer, settings, _renewer, messageFlow);
        }
    }
}