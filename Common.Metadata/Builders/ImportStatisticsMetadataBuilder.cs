using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Builders
{
    public class ImportStatisticsMetadataBuilder<T, TDto> : MetadataElementBuilder<ImportStatisticsMetadataBuilder<T, TDto>, ImportStatisticsMetadata<T, TDto>>
    {
        private Type _statisticsDtoType;
        private Func<TDto, FindSpecification<T>> _findSpecificationProvider;
        private IMapSpecification<TDto, IReadOnlyCollection<T>> _mapSpecification;

        protected override ImportStatisticsMetadata<T, TDto> Create()
        {
            return new ImportStatisticsMetadata<T, TDto>(_statisticsDtoType, _findSpecificationProvider, _mapSpecification, Features);
        }

        public ImportStatisticsMetadataBuilder<T, TDto> HasSource<TStatisticsDto>(IMapSpecification<TDto, IReadOnlyCollection<T>> mapSpecification)
            where TStatisticsDto : IDataTransferObject
        {
            _statisticsDtoType = typeof(TStatisticsDto);
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