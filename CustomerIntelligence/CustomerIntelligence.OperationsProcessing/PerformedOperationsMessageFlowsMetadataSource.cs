using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.OperationsProcessing.Final;
using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.CustomerIntelligence.OperationsProcessing.Primary;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.Replication.OperationsProcessing.Transports.CorporateBus;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;

namespace NuClear.CustomerIntelligence.OperationsProcessing
{
    public sealed class PerformedOperationsMessageFlowsMetadataSource : MetadataSourceBase<MetadataMessageFlowsIdentity>
    {
        private static readonly HierarchyMetadata MetadataRoot =
            PerformedOperations.Flows
                               .Primary(
                                        MessageFlowMetadata.Config.For<ImportFactsFromErmFlow>()
                                                           .Receiver<ServiceBusMessageReceiverTelemetryDecorator>()
                                                           .Accumulator<ImportFactsFromErmAccumulator>()
                                                           .Handler<ImportFactsFromErmHandler>()
                                                           .To.Primary().Flow<ImportFactsFromErmFlow>().Connect(),

                                        MessageFlowMetadata.Config.For<ImportFactsFromBitFlow>()
                                                           .Receiver<CorporateBusOperationsReceiver>()
                                                           .Accumulator<ImportFactsFromBitAccumulator>()
                                                           .Handler<ImportFactsFromBitHandler>()
                                                           .To.Primary().Flow<ImportFactsFromBitFlow>().Connect(),

                                        MessageFlowMetadata.Config.For<CommonEventsFlow>()
                                                           .Receiver<ServiceBusMessageReceiverTelemetryDecorator>()
                                                           .Accumulator<CommonEventsAccumulator>()
                                                           .Handler<AggregateCommandsHandler>()
                                                           .To.Primary().Flow<CommonEventsFlow>().Connect(),

                                        MessageFlowMetadata.Config.For<StatisticsEventsFlow>()
                                                           .Receiver<ServiceBusMessageReceiverTelemetryDecorator>()
                                                           .Accumulator<ProjectStatisticsAggregateEventsAccumulator>()
                                                           .Handler<ProjectStatisticsAggregateCommandsHandler>()
                                                           .To.Primary().Flow<StatisticsEventsFlow>().Connect());

        public PerformedOperationsMessageFlowsMetadataSource()
        {
            Metadata = new Dictionary<Uri, IMetadataElement> { { MetadataRoot.Identity.Id, MetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}
