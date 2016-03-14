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

namespace NuClear.CustomerIntelligence.OperationsProcessing
{
    public sealed class PerformedOperationsMessageFlowsMetadataSource : MetadataSourceBase<MetadataMessageFlowsIdentity>
    {
        private static readonly HierarchyMetadata MetadataRoot =
            PerformedOperations.Flows
                               .Primary(

                                   MessageFlowMetadata.Config.For<ImportFactsFromErmFlow>()
                                                      .Accumulator<ImportFactsFromErmAccumulator>()
                                                      .Handler<ImportFactsFromErmHandler>()
                                                      .To.Primary().Flow<ImportFactsFromErmFlow>().Connect()
                                                      .To.Final().Flow<AggregatesFlow>().Connect()
                                                      .To.Final().Flow<StatisticsFlow>().Connect(),

                                   MessageFlowMetadata.Config.For<ImportFactsFromBitFlow>()
                                                      .Accumulator<ImportFactsFromBitAccumulator>()
                                                      .Handler<ImportFactsFromBitHandler>()
                                                      .To.Primary().Flow<ImportFactsFromBitFlow>().Connect()
                                                      .To.Final().Flow<StatisticsFlow>().Connect()
                )
                               .Final(

                                   MessageFlowMetadata.Config.For<AggregatesFlow>()
                                                      .Accumulator<AggregateOperationAccumulator<AggregatesFlow>>()
                                                      .Handler<AggregateOperationAggregatableMessageHandler>()
                                                      .To.Final().Flow<AggregatesFlow>().Connect(),

                                   MessageFlowMetadata.Config.For<StatisticsFlow>()
                                                      .Accumulator<AggregateOperationAccumulator<StatisticsFlow>>()
                                                      .Handler<StatisticsAggregatableMessageHandler>()
                                                      .To.Final().Flow<StatisticsFlow>().Connect()

                );

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
