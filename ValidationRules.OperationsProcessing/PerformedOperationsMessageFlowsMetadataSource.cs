using System;
using System.Collections.Generic;

using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;
using NuClear.ValidationRules.OperationsProcessing.Primary;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;
using NuClear.ValidationRules.OperationsProcessing.AfterFinal;
using NuClear.ValidationRules.OperationsProcessing.Final;
using NuClear.ValidationRules.OperationsProcessing.Transports;

namespace NuClear.ValidationRules.OperationsProcessing
{
    // todo: переименовать во что-нибудь типа DataFlowMetadataSource, но одинаково в Ci и Vr
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

                                        MessageFlowMetadata.Config.For<CommonEventsFlow>()
                                                           .Receiver<ServiceBusMessageReceiverTelemetryDecorator>()
                                                           .Accumulator<AggregateCommandsAccumulator>()
                                                           .Handler<AggregateCommandsHandler>()
                                                           .To.Primary().Flow<CommonEventsFlow>().Connect(),

                                        MessageFlowMetadata.Config.For<MessagesFlow>()
                                                           .Receiver<BatchingServiceBusMessageReceiverTelemetryDecorator>()
                                                           .Accumulator<MessageCommandsAccumulator>()
                                                           .Handler<MessageCommandsHandler>()
                                                           .To.Primary().Flow<MessagesFlow>().Connect()
                                       );

        public PerformedOperationsMessageFlowsMetadataSource()
        {
            Metadata = new Dictionary<Uri, IMetadataElement> { { MetadataRoot.Identity.Id, MetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}
