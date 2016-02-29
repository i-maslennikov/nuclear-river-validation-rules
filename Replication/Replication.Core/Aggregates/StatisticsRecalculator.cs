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
                throw new NotSupportedException(string.Format("Metadata for identity '{0}' cannot be found.", typeof(StatisticsRecalculationMetadataIdentity).Name));
            }

            var metadata = metadataSet.Metadata.Values.SelectMany(x => x.Elements).Single();
            var processor = _statisticsProcessorFactory.Create(metadata);

            using (Probe.Create("Recalculate Statistics Operations"))
            {
                foreach (var batch in operations.GroupBy(x => x.EntityId.ProjectId))
                {
                    processor.Execute(batch.ToArray());
                }
            }
        }
    }
}
