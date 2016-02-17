using System;
using System.Collections.Generic;

using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;
using NuClear.ValidationRules.OperationsProcessing.Primary;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.Replication.OperationsProcessing.Final;
using NuClear.ValidationRules.OperationsProcessing.Final;

namespace NuClear.ValidationRules.OperationsProcessing
{
    // todo: переименовать во что-нибудь типа DataFlowMetadataSource, но одинаково в Ci и Vr
    public sealed class PerformedOperationsMessageFlowsMetadataSource : MetadataSourceBase<MetadataMessageFlowsIdentity>
    {
        private static readonly HierarchyMetadata MetadataRoot =
            PerformedOperations.Flows
                               .Primary(
                                   MessageFlowMetadata.Config.For<ImportFactsFromErmFlow>()
                                                      .Accumulator<ImportFactsFromErmAccumulator>()
                                                      .Handler<ImportFactsFromErmHandler>()
                                                      .To.Primary().Flow<ImportFactsFromErmFlow>().Connect()
                                                      .To.Final().Flow<AggregatesFlow>().Connect(),

                                   MessageFlowMetadata.Config.For<ImportFactsFromOrderValidationConfigFlow>()
                                                      .Accumulator<ImportFactsFromOrderValidationConfigAccumulator>()
                                                      .Handler<ImportFactsFromOrderValidationConfigHandler>()
                                                      .To.Primary().Flow<ImportFactsFromOrderValidationConfigFlow>().Connect()
                                )
                                .Final(
                                   MessageFlowMetadata.Config.For<AggregatesFlow>()
                                                      .Accumulator<AggregateOperationAccumulator<AggregatesFlow>>()
                                                      .Handler<AggregateOperationAggregatableMessageHandler>()
                                                      .To.Final().Flow<AggregatesFlow>().Connect()
                                );

        public PerformedOperationsMessageFlowsMetadataSource()
        {
            Metadata = new Dictionary<Uri, IMetadataElement> { { MetadataRoot.Identity.Id, MetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}
