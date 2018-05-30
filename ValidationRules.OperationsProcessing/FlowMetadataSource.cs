﻿using System;
using System.Collections.Generic;

using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.OperationsProcessing.Transports.Kafka;
using NuClear.ValidationRules.OperationsProcessing.AggregatesFlow;
using NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow;
using NuClear.ValidationRules.OperationsProcessing.FactsFlow;
using NuClear.ValidationRules.OperationsProcessing.MessagesFlow;
using NuClear.ValidationRules.OperationsProcessing.Transports;
using NuClear.ValidationRules.OperationsProcessing.RulesetFactsFlow;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public sealed class FlowMetadataSource : MetadataSourceBase<MetadataMessageFlowsIdentity>
    {
        private static readonly HierarchyMetadata MetadataRoot =
            PerformedOperations.Flows
                               .Primary(
                                        MessageFlowMetadata.Config.For<AmsFactsFlow.AmsFactsFlow>()
                                                           .Receiver<BatchingKafkaReceiverTelemetryDecorator<AmsFactsFlowTelemetryPublisher>>()
                                                           .Accumulator<AmsFactsFlowAccumulator>()
                                                           .Handler<AmsFactsFlowHandler>()
                                                           .To.Primary().Flow<AmsFactsFlow.AmsFactsFlow>().Connect(),

                                        MessageFlowMetadata.Config.For<RulesetFactsFlow.RulesetFactsFlow>()
                                                           .Receiver<KafkaReceiver>()
                                                           .Accumulator<RulesetFactsFlowAccumulator>()
                                                           .Handler<RulesetFactsFlowHandler>()
                                                           .To.Primary().Flow<RulesetFactsFlow.RulesetFactsFlow>().Connect(),

                                        MessageFlowMetadata.Config.For<FactsFlow.FactsFlow>()
                                                           .Receiver<BatchingServiceBusMessageReceiverTelemetryDecorator<FactsFlowTelemetryPublisher>>()
                                                           .Accumulator<FactsFlowAccumulator>()
                                                           .Handler<FactsFlowHandler>()
                                                           .To.Primary().Flow<FactsFlow.FactsFlow>().Connect(),

                                        MessageFlowMetadata.Config.For<AggregatesFlow.AggregatesFlow>()
                                                           .Receiver<BatchingServiceBusMessageReceiverTelemetryDecorator<AggregatesFlowTelemetryPublisher>>()
                                                           .Accumulator<AggregatesFlowAccumulator>()
                                                           .Handler<AggregatesFlowHandler>()
                                                           .To.Primary().Flow<AggregatesFlow.AggregatesFlow>().Connect(),

                                        MessageFlowMetadata.Config.For<MessagesFlow.MessagesFlow>()
                                                           .Receiver<BatchingServiceBusMessageReceiverTelemetryDecorator<MessagesFlowTelemetryPublisher>>()
                                                           .Accumulator<MessagesFlowAccumulator>()
                                                           .Handler<MessagesFlowHandler>()
                                                           .To.Primary().Flow<MessagesFlow.MessagesFlow>().Connect()
                                       );

        public FlowMetadataSource()
        {
            Metadata = new Dictionary<Uri, IMetadataElement> { { MetadataRoot.Identity.Id, MetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}
