using System;
using System.Collections.Generic;

using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.ValidationRules.OperationsProcessing.AggregatesFlow;
using NuClear.ValidationRules.OperationsProcessing.Facts.AmsFactsFlow;
using NuClear.ValidationRules.OperationsProcessing.Facts.ErmFactsFlow;
using NuClear.ValidationRules.OperationsProcessing.Facts.RulesetFactsFlow;
using NuClear.ValidationRules.OperationsProcessing.MessagesFlow;
using NuClear.ValidationRules.OperationsProcessing.Transports;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public sealed class FlowMetadataSource : MetadataSourceBase<MetadataMessageFlowsIdentity>
    {
        private static readonly HierarchyMetadata MetadataRoot =
            PerformedOperations.Flows
                               .Primary(
                                        MessageFlowMetadata.Config.For<AmsFactsFlow>()
                                                           .Receiver<BatchingKafkaReceiverTelemetryDecorator<AmsFactsFlowTelemetryPublisher>>()
                                                           .Accumulator<AmsFactsFlowAccumulator>()
                                                           .Handler<AmsFactsFlowHandler>()
                                                           .To.Primary().Flow<AmsFactsFlow>().Connect(),

                                        MessageFlowMetadata.Config.For<RulesetFactsFlow>()
                                                           .Receiver<BatchingKafkaReceiverTelemetryDecorator<RulesetFactsFlowTelemetryPublisher>>()
                                                           .Accumulator<RulesetFactsFlowAccumulator>()
                                                           .Handler<RulesetFactsFlowHandler>()
                                                           .To.Primary().Flow<RulesetFactsFlow>().Connect(),

                                        MessageFlowMetadata.Config.For<ErmFactsFlow>()
                                                           .Receiver<BatchingServiceBusMessageReceiverTelemetryDecorator<ErmFactsFlowTelemetryPublisher>>()
                                                           .Accumulator<ErmFactsFlowAccumulator>()
                                                           .Handler<ErmFactsFlowHandler>()
                                                           .To.Primary().Flow<ErmFactsFlow>().Connect(),

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
