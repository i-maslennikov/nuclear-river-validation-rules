using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Provider;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Identities;
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

        public void Recalculate(IEnumerable<RecalculateStatisticsOperation> operations)
        {
            MetadataSet metadataSet;
            if (!_metadataProvider.TryGetMetadata<StatisticsRecalculationMetadataIdentity>(out metadataSet))
            {
                throw new NotSupportedException($"Metadata for identity '{typeof(StatisticsRecalculationMetadataIdentity).Name}' cannot be found.");
            }

            var batches = operations.GroupBy(x => x.EntityId.ProjectId).ToArray();
            using (Probe.Create("Recalculate Statistics Operations"))
            {
                var metadata = metadataSet.Metadata.Values.SelectMany(x => x.Elements).ToArray();
                foreach (var element in metadata)
                {
                    var processor = _statisticsProcessorFactory.Create(element);

                    foreach (var batch in batches)
                    {
                        processor.Execute(batch.ToArray());
                    }
                }
            }
        }
}
}
