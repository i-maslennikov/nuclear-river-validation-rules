using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.ServiceBus.API;
using NuClear.OperationsLogging.API;
using NuClear.OperationsLogging.Transports.ServiceBus;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus.Factories;
using NuClear.ValidationRules.OperationsProcessing.AggregatesFlow;
using NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow;
using NuClear.ValidationRules.OperationsProcessing.FactsFlow;
using NuClear.ValidationRules.OperationsProcessing.MessagesFlow;
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
            var messageFlows = events.Cast<FlowEvent>().Select(x => x.Flow).Distinct();

            var strategies = messageFlows.Aggregate(new List<IEventLoggingStrategy<TEvent>>(),
            (list, flow) =>
            {
                IEventLoggingStrategy<TEvent> strategy;

                if (Equals(flow, FactsFlow.Instance) ||
                    Equals(flow, AmsFactsFlow.Instance))
                {
                    strategy = ResolveServiceBusStrategy<TEvent>(AggregatesFlow.Instance);
                } else if (Equals(flow, AggregatesFlow.Instance))
                {
                    strategy = ResolveServiceBusStrategy<TEvent>(MessagesFlow.Instance);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(flow), flow, "Unexpected flow");
                }

                list.Add(strategy);
                return list;
            });

            return strategies;
        }

        private IEventLoggingStrategy<TEvent> ResolveServiceBusStrategy<TEvent>(IMessageFlow flow)
        {
            var serviceBusSettings = _settingsFactory.CreateSenderSettings(flow);
            var strategy = _unityContainer.Resolve<SessionlessServiceBusEventLoggingStrategy<TEvent>>(new DependencyOverride<IServiceBusMessageSenderSettings>(serviceBusSettings));
            return strategy;
        }
    }
}
