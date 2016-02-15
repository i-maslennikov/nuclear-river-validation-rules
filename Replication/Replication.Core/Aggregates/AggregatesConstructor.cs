using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Identities;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class AggregatesConstructor : IAggregatesConstructor
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IAggregateProcessorFactory _aggregateProcessorFactory;

        public AggregatesConstructor(IMetadataProvider metadataProvider, IAggregateProcessorFactory aggregateProcessorFactory)
        {
            _metadataProvider = metadataProvider;
            _aggregateProcessorFactory = aggregateProcessorFactory;
        }

        public void Construct(IEnumerable<AggregateOperation> operations)
        {
            using (Probe.Create("ETL2 Transforming"))
            {
                var slices = operations.GroupBy(x => new { Operation = x.GetType(), x.AggregateType })
                                       .OrderByDescending(x => x.Key.Operation, new AggregateOperationPriorityComparer());

                foreach (var slice in slices)
                {
                    var operation = slice.Key.Operation;
                    var aggregateType = slice.Key.AggregateType;

                    IMetadataElement aggregateMetadata;
                    var metadataId = ReplicationMetadataIdentity.Instance.Id.WithRelative(new Uri($"Aggregates/{aggregateType.Name}", UriKind.Relative));
                    if (!_metadataProvider.TryGetMetadata(metadataId, out aggregateMetadata))
                    {
                        throw new NotSupportedException($"The aggregate of type '{aggregateType}' is not supported.");
                    }

                    var aggregateIds = slice.Select(x => x.AggregateId).Distinct().ToArray();
                    using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                                                                  new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
                    {
                        using (Probe.Create("ETL2 Transforming", aggregateType.Name))
                        {
                            var processor = _aggregateProcessorFactory.Create(aggregateMetadata);

                            if (operation == typeof(InitializeAggregate))
                            {
                                processor.Initialize(aggregateIds);
                            }
                            else if (operation == typeof(RecalculateAggregate))
                            {
                                processor.Recalculate(aggregateIds);
                            }
                            else if (operation == typeof(DestroyAggregate))
                            {
                                processor.Destroy(aggregateIds);
                            }
                            else
                            {
                                throw new InvalidOperationException($"The command of type {operation.Name} is not supported");
                            }
                        }

                        transaction.Complete();
                    }
                }
            }
        }
    }
}

