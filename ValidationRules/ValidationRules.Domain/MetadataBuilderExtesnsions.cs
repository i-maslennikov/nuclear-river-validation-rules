using System;
using System.Collections.Generic;

using NuClear.River.Common.Metadata.Builders;
using NuClear.River.Common.Metadata.Features;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.ValidationRules.Domain
{
    public static class MetadataBuilderExtesnsions
    {
        public static ImportStatisticsMetadataBuilder<T, TDto> FakeOperationsProvider<T, TDto>(this ImportStatisticsMetadataBuilder<T, TDto> builder)
        {
            Func<TDto, IReadOnlyCollection<IOperation>> projector = x => new IOperation[0];
            var specification = new MapSpecification<TDto, IReadOnlyCollection<IOperation>>(projector);
            var feature = new MapSpecificationFeature<TDto, IReadOnlyCollection<IOperation>>(specification);
            return builder.WithFeatures(feature);
        }
    }
}