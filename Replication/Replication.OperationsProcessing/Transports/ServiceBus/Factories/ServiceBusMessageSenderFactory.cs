using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.ServiceBus;
using NuClear.Messaging.Transports.ServiceBus.API;
using NuClear.Tracing.API;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus.Factories
{
    public sealed class ServiceBusMessageSenderFactory
    {
        private readonly IServiceBusSettingsFactory _settingsFactory;
        private readonly ITracer _tracer;

        public ServiceBusMessageSenderFactory(ITracer tracer, IServiceBusSettingsFactory settingsFactory)
        {
            _settingsFactory = settingsFactory;
            _tracer = tracer;
        }

        public IServiceBusMessageSender Create(IMessageFlow targetFlow)
        {
            var settings = _settingsFactory.CreateSenderSettings(targetFlow);
            return new ServiceBusMessageSender(settings, _tracer);
        }
    }
}
