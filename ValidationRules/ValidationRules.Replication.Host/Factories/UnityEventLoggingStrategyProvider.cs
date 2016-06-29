using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.ServiceBus.API;
using NuClear.OperationsLogging.API;
using NuClear.OperationsLogging.Transports.ServiceBus;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus.Factories;
using NuClear.Telemetry;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.Replication.Host.Factories
{
    public sealed class UnityEventLoggingStrategyProvider : IEventLoggingStrategyProvider
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IServiceBusSettingsFactory _settingsFactory;

        public UnityEventLoggingStrategyProvider(IUnityContainer unityContainer, IServiceBusSettingsFactory settingsFactory)
        {
            _unityContainer = unityContainer;
            _settingsFactory = settingsFactory;
        }

        public IReadOnlyCollection<IEventLoggingStrategy<TEvent>> Get<TEvent>(IReadOnlyCollection<TEvent> events)
        {
            var flows = new Dictionary<IMessageFlow, IFlowAspect<TEvent>>
                {
                    { CommonEventsFlow.Instance, _unityContainer.Resolve<CommonEventsFlowAspect<TEvent>>() },
                    { MessagesFlow.Instance, _unityContainer.Resolve<MessagesFlowAspect<TEvent>>() }
                };

            return flows.Select(flow => new { Strategy = ResolveServiceBusStrategy<TEvent>(flow.Key), Aspect = flow.Value })
                        .Select(x => DecorateStrategy(x.Strategy, x.Aspect))
                        .ToArray();
        }

        private IEventLoggingStrategy<TEvent> ResolveServiceBusStrategy<TEvent>(IMessageFlow flow)
        {
            var serviceBusSettings = _settingsFactory.CreateSenderSettings(flow);
            var strategy = _unityContainer.Resolve<SessionlessServiceBusEventLoggingStrategy<TEvent>>(new DependencyOverride<IServiceBusMessageSenderSettings>(serviceBusSettings));
            return strategy;
        }

        private IEventLoggingStrategy<TEvent> DecorateStrategy<TEvent>(IEventLoggingStrategy<TEvent> strategy, IFlowAspect<TEvent> flow)
        {
            return new EventLoggingStrategyDecorator<TEvent>(strategy, flow);
        }

        private class CommonEventsFlowAspect<TEvent> : IFlowAspect<TEvent>
        {
            private readonly ITelemetryPublisher _telemetryPublisher;

            public CommonEventsFlowAspect(ITelemetryPublisher telemetryPublisher)
            {
                _telemetryPublisher = telemetryPublisher;
            }

            public bool ShouldEventBeLogged(TEvent @event)
                => !(@event is AggregatesStateIncrementedEvent || @event is AggregatesBatchProcessedEvent);

            public void ReportMessageLoggedCount(long count)
                => _telemetryPublisher.Publish<AggregateEnqueuedOperationCountIdentity>(count);

        }

        private class MessagesFlowAspect<TEvent> : IFlowAspect<TEvent>
        {
            private readonly ITelemetryPublisher _telemetryPublisher;

            public MessagesFlowAspect(ITelemetryPublisher telemetryPublisher)
            {
                _telemetryPublisher = telemetryPublisher;
            }

            public bool ShouldEventBeLogged(TEvent @event)
                => @event is AggregatesStateIncrementedEvent || @event is AggregatesBatchProcessedEvent;

            public void ReportMessageLoggedCount(long count)
            {
                // todo: можно отправлять статистику по наполнению очереди пересчёта сообщений, кажется, раньше её не было
            }
        }
    }
}
