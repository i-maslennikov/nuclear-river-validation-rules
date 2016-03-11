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
            var processors = CreateProcessor();

            using (Probe.Create("Recalculate Statistics Operations"))
            {
                foreach (var slice in _slicer.Slice(operations.Select(o => o.Context)))
                {
                    foreach (var processor in processors)
                    {
                        processor.RecalculateStatistics(slice);
                    }
                }
            }
        }

        private IReadOnlyCollection<IStatisticsProcessor> CreateProcessor()
        {
            MetadataSet metadataSet;
            if (!_metadataProvider.TryGetMetadata<StatisticsRecalculationMetadataIdentity>(out metadataSet))
            {
                throw new NotSupportedException($"Metadata for identity '{typeof(StatisticsRecalculationMetadataIdentity).Name}' cannot be found.");
            }

            var metadata = metadataSet.Metadata.Values.SelectMany(x => x.Elements).ToArray();
            return metadata.Select(_statisticsProcessorFactory.Create).ToArray();
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
