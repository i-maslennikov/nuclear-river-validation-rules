using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Identities;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.Core.Aggregates
{
    // Нужен?
    public class StatisticsRecalculator : IStatisticsRecalculator
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IStatisticsProcessorFactory _statisticsProcessorFactory;

        public StatisticsRecalculator(IMetadataProvider metadataProvider, IStatisticsProcessorFactory statisticsProcessorFactory)
        {
            _metadataProvider = metadataProvider;
            _statisticsProcessorFactory = statisticsProcessorFactory;
        }

        public void Execute(IReadOnlyCollection<IOperation> commands)
        {
            using (Probe.Create("Recalculate Statistics Operations"))
            {
                var commandsByType = commands.GroupBy(x => x.GetType()).OrderBy(x => x.Key);

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                                              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
                {
                    foreach (var group in commandsByType)
                    {
                        if (group.Key == typeof(RecalculateStatisticsOperation))
                        {
                            Execute(group.Cast<RecalculateStatisticsOperation>());
                        }
                        else
                        {
                            throw new InvalidOperationException($"The command of type {group.Key.Name} is not supported");
                        }
                    }

                    transaction.Complete();
                }
            }
        }

        private void Execute(IEnumerable<RecalculateStatisticsOperation> enumerable)
        {
            var processor = CreateProcessors();
            foreach (var slice in enumerable.GroupBy(x => x.ProjectId))
            {
                foreach (var statisticsProcessor in processor)
                {
                    statisticsProcessor.Execute(slice.ToArray());
                }
            }
        }

        private IReadOnlyCollection<IStatisticsProcessor> CreateProcessors()
        {
            IMetadataElement aggregateMetadata;
            var metadataId = ReplicationMetadataIdentity.Instance.Id.WithRelative(new Uri($"Statistics/ProjectStatistics", UriKind.Relative));
            if (!_metadataProvider.TryGetMetadata(metadataId, out aggregateMetadata))
            {
                throw new NotSupportedException($"The aggregate of type 'ProjectStatistics' is not supported.");
            }

            MetadataSet metadataSet;
            if (!_metadataProvider.TryGetMetadata<ReplicationMetadataIdentity>(out metadataSet))
            {
                throw new NotSupportedException($"Metadata for identity '{typeof(ReplicationMetadataIdentity).Name}' cannot be found.");
            }

            var metadata = metadataSet.Metadata.Values.SelectMany(x => x.Elements).ToArray();
            var processors = metadata.Select(_statisticsProcessorFactory.Create).ToArray();
            return processors;
        }
    }
}
