using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Metamodeling.Provider;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Identities;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.Core.Aggregates
{
    public class StatisticsRecalculator : IStatisticsRecalculator
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IStatisticsProcessorFactory _statisticsProcessorFactory;

        public StatisticsRecalculator(IMetadataProvider metadataProvider, IStatisticsProcessorFactory statisticsProcessorFactory)
        {
            _metadataProvider = metadataProvider;
            _statisticsProcessorFactory = statisticsProcessorFactory;
        }

        public void Recalculate(IReadOnlyCollection<IOperation> commands)
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
            MetadataSet metadataSet;
            if (!_metadataProvider.TryGetMetadata<StatisticsRecalculationMetadataIdentity>(out metadataSet))
            {
                throw new NotSupportedException($"Metadata for identity '{typeof(StatisticsRecalculationMetadataIdentity).Name}' cannot be found.");
            }

            var metadata = metadataSet.Metadata.Values.SelectMany(x => x.Elements).ToArray();
            var processors = metadata.Select(_statisticsProcessorFactory.Create).ToArray();
            return processors;
        }
    }
}
