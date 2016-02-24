using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Provider;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Context;
using NuClear.River.Common.Metadata.Identities;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.Core.Aggregates
{
    public class StatisticsRecalculator : IStatisticsRecalculator
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IStatisticsProcessorFactory _statisticsProcessorFactory;
        private readonly ISlicer<StatisticsProcessorSlice> _slicer;

        public StatisticsRecalculator(IMetadataProvider metadataProvider, IStatisticsProcessorFactory statisticsProcessorFactory)
        {
            _metadataProvider = metadataProvider;
            _statisticsProcessorFactory = statisticsProcessorFactory;
            _slicer = new StatisticsRecalculationSlicer();
        }

        public void Recalculate(IEnumerable<RecalculateStatisticsOperation> operations)
        {
            var processor = CreateProcessor();

            using (Probe.Create("Recalculate Statistics Operations"))
            {
                foreach (var slice in _slicer.Slice(operations.Select(o => o.Context)))
                {
                    processor.RecalculateStatistics(slice);
                }
            }
        }

        private IStatisticsProcessor CreateProcessor()
        {
            MetadataSet metadataSet;
            if (!_metadataProvider.TryGetMetadata<StatisticsRecalculationMetadataIdentity>(out metadataSet))
            {
                throw new NotSupportedException(string.Format("Metadata for identity '{0}' cannot be found.", typeof(StatisticsRecalculationMetadataIdentity).Name));
            }

            var metadata = metadataSet.Metadata.Values.SelectMany(x => x.Elements).Single();
            return _statisticsProcessorFactory.Create(metadata);
        }

        public sealed class StatisticsRecalculationSlicer : ISlicer<StatisticsProcessorSlice>
            {
            public IEnumerable<StatisticsProcessorSlice> Slice(IEnumerable<Predicate> predicates)
                {
                return predicates.GroupBy(p => PredicateProperty.ProjectId.GetValue(p))
                                 .Select(group => group.Any(p => PredicateProperty.Type.GetValue(p) == PredicateFactory.Type.ByProject)
                                                      ? new StatisticsProcessorSlice { ProjectId = group.Key, CategoryIds = default(IReadOnlyCollection<long>) }
                                                      : new StatisticsProcessorSlice { ProjectId = group.Key, CategoryIds = group.Select(p => PredicateProperty.CategoryId.GetValue(p)).Distinct().ToArray() });
            }
        }
    }
}
