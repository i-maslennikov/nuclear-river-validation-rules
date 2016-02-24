using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.River.Common.Metadata.Elements;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Builders
{
    public class ImportStatisticsMetadataBuilder<T, TDto> : MetadataElementBuilder<ImportStatisticsMetadataBuilder<T, TDto>, ImportStatisticsMetadata<T, TDto>>
    {
        private Func<TDto, FindSpecification<T>> _findSpecificationProvider;
        private IMapSpecification<TDto, IReadOnlyCollection<T>> _mapSpecification;

        protected override ImportStatisticsMetadata<T, TDto> Create()
        {
            return new ImportStatisticsMetadata<T, TDto>(_findSpecificationProvider, _mapSpecification, Features);
        }

        public ImportStatisticsMetadataBuilder<T, TDto> HasSource(IMapSpecification<TDto, IReadOnlyCollection<T>> mapSpecification)
        {
            _mapSpecification = mapSpecification;
            return this;
        }

        public ImportStatisticsMetadataBuilder<T, TDto> Aggregated(Func<TDto, FindSpecification<T>> findSpecificationProvider)
        {
            _findSpecificationProvider = findSpecificationProvider;
            return this;
        }
    }
}