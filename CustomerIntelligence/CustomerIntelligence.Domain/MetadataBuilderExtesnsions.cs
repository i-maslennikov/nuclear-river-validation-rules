using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Model;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Builders;
using NuClear.River.Common.Metadata.Features;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain
{
    public static class MetadataBuilderExtesnsions
    {
        public static FactMetadataBuilder<T> LeadsToStatisticsCalculation<T>(this FactMetadataBuilder<T> builder, Func<FindSpecification<T>, MapSpecification<IQuery, IEnumerable<Tuple<long, long?>>>> provider)
            where T : class, IIdentifiable<long>
        {
            MapToObjectsSpecProvider<T, IOperation> mapSpecificationProvider =
                specification => new MapSpecification<IQuery, IEnumerable<IOperation>>(
                                     q => provider.Invoke(specification)
                                                  .Map(q)
                                                  .Select(tuple => new RecalculateStatisticsOperation
                                                      {
                                                          ProjectId = tuple.Item1,
                                                          CategoryId = tuple.Item2
                                                      }));

            return builder.WithFeatures(new DependentStatisticsFeature<T, long>(mapSpecificationProvider, DefaultIdentityProvider.Instance));
        }

        public static ImportStatisticsMetadataBuilder<T, TDto> LeadsToProjectStatisticsCalculation<T, TDto>(this ImportStatisticsMetadataBuilder<T, TDto> builder)
            where TDto : IBitDto
        {
            Func<TDto, IReadOnlyCollection<IOperation>> projector = x => new [] { new RecalculateStatisticsOperation { ProjectId = x.ProjectId } };
            var specification = new MapSpecification<TDto, IReadOnlyCollection<IOperation>>(projector);
            var feature = new MapSpecificationFeature<TDto, IReadOnlyCollection<IOperation>>(specification);
            return builder.WithFeatures(feature);
        }
    }
}