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
using NuClear.Replication.OperationsProcessing.Final;
using NuClear.Replication.OperationsProcessing.Transports.CorporateBus;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;

namespace NuClear.CustomerIntelligence.OperationsProcessing
{
    public sealed class PerformedOperationsMessageFlowsMetadataSource : MetadataSourceBase<MetadataMessageFlowsIdentity>
    {
        private static readonly HierarchyMetadata MetadataRoot =
            PerformedOperations.Flows
                               .Primary(
                                        MessageFlowMetadata.Config.For<ImportFactsFromErmFlow>()
                                                           .Receiver<ServiceBusOperationsReceiverTelemetryDecorator>()
                                                           .Accumulator<ImportFactsFromErmAccumulator>()
                                                           .Handler<ImportFactsFromErmHandler>()
                                                           .To.Primary().Flow<ImportFactsFromErmFlow>().Connect(),

                                        MessageFlowMetadata.Config.For<ImportFactsFromBitFlow>()
                                                           .Receiver<CorporateBusOperationsReceiver>()
                                                           .Accumulator<ImportFactsFromBitAccumulator>()
                                                           .Handler<ImportFactsFromBitHandler>()
                                                           .To.Primary().Flow<ImportFactsFromBitFlow>().Connect(),

                                        MessageFlowMetadata.Config.For<AggregatesFlow>()
                                                           .Receiver<SqlStoreReceiverTelemetryDecorator>()
                                                           .Accumulator<AggregateOperationAccumulator<AggregatesFlow>>()
                                                           .Handler<AggregateOperationAggregatableMessageHandler>()
                                                           .To.Primary().Flow<AggregatesFlow>().Connect(),

                                        MessageFlowMetadata.Config.For<StatisticsFlow>()
                                                           .Receiver<SqlStoreReceiverTelemetryDecorator>()
                                                           .Accumulator<AggregateOperationAccumulator<StatisticsFlow>>()
                                                           .Handler<StatisticsAggregatableMessageHandler>()
                                                           .To.Primary().Flow<StatisticsFlow>().Connect());

        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public PerformedOperationsMessageFlowsMetadataSource()
        {
            _metadata = new Dictionary<Uri, IMetadataElement> { { MetadataRoot.Identity.Id, MetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }
    }
}
