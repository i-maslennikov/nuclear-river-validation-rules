using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.ServiceBus.API;
using NuClear.OperationsLogging.API;
using NuClear.OperationsLogging.Transports.ServiceBus;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus.Factories;
using NuClear.Telemetry;

namespace NuClear.CustomerIntelligence.Replication.Host.Factories
{
    public sealed class UnityEventLoggingStrategyProvider : IEventLoggingStrategyProvider
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IServiceBusSettingsFactory _settingsFactory;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public UnityEventLoggingStrategyProvider(IUnityContainer unityContainer, IServiceBusSettingsFactory settingsFactory, ITelemetryPublisher telemetryPublisher)
        {
            _unityContainer = unityContainer;
            _settingsFactory = settingsFactory;
            _telemetryPublisher = telemetryPublisher;
        }

        public IReadOnlyCollection<IEventLoggingStrategy<TEvent>> Get<TEvent>(IReadOnlyCollection<TEvent> events)
        {
            var flows = new IMessageFlow[] { CommonEventsFlow.Instance, StatisticsEventsFlow.Instance };
            return flows.Select(flow => new { Flow = flow, Strategy = ResolveServiceBusStrategy<TEvent>(flow) })
                        .Select(x => DecorateStrategy(x.Strategy, x.Flow))
                        .ToArray();
        }

        private IEventLoggingStrategy<TEvent> ResolveServiceBusStrategy<TEvent>(IMessageFlow flow)
        {
            var serviceBusSettings = _settingsFactory.CreateSenderSettings(flow);
            var strategy = _unityContainer.Resolve<ServiceBusEventLoggingStrategy<TEvent>>(new DependencyOverride<IServiceBusMessageSenderSettings>(serviceBusSettings));
            return strategy;
        }

        private IEventLoggingStrategy<TEvent> DecorateStrategy<TEvent>(IEventLoggingStrategy<TEvent> strategy, IMessageFlow flow)
        {
            return new EventLoggingStrategyDecorator<TEvent>(strategy, CreateFilter<TEvent>(flow), CreateReporter(flow));
        }

        private Func<TEvent, bool> CreateFilter<TEvent>(IMessageFlow flow)
        {
            return e => CheckIfEventMatchesFlow(e, flow);
        }

        private Action<long> CreateReporter(IMessageFlow flow)
        {
            if (Equals(flow, CommonEventsFlow.Instance))
            {
                return count => _telemetryPublisher.Publish<AggregateEnqueuedOperationCountIdentity>(count);
            }

            if (Equals(flow, StatisticsEventsFlow.Instance))
            {
                return count => _telemetryPublisher.Publish<StatisticsEnqueuedOperationCountIdentity>(count);
            }

            throw new ArgumentException($"Unsupported flow {flow}", nameof(flow));
        }

        public bool CheckIfEventMatchesFlow<TEvent>(TEvent @event, IMessageFlow flow)
        {
            if (Equals(flow, StatisticsEventsFlow.Instance))
            {
                return @event is DataObjectReplacedEvent || @event is RelatedDataObjectOutdatedEvent<StatisticsKey>;
            }

            if (Equals(flow, CommonEventsFlow.Instance))
            {
                // Инвертировать условие для StatisticsEventsFlow не лучшая идея в общем случае,
                // но лучшее решение для того, чтобы гарантировать сохранение логики (в которой, кажется, есть ошибка)
                return !(@event is DataObjectReplacedEvent || @event is RelatedDataObjectOutdatedEvent<StatisticsKey>);
            }

            throw new ArgumentException($"Unsupported flow {flow}", nameof(flow));
        }
    }
}
