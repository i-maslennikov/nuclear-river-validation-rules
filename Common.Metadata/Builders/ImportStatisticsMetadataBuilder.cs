using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.River.Common.Metadata.Elements;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Builders
{
    public class ImportDocumentMetadataBuilder<TDto> : MetadataElementBuilder<ImportDocumentMetadataBuilder<TDto>, ImportDocumentMetadata<TDto>>
    {
        protected override ImportDocumentMetadata<TDto> Create()
        {
            return new ImportDocumentMetadata<TDto>(Features);
        }

        /// <summary>
        /// Добавляет описание импорта из документа в таблицу фактов
        /// </summary>
        /// <typeparam name="TFact"></typeparam>
        /// <param name="findSpecificationProvider">Определяет, какие факты должны быть удалены</param>
        /// <param name="mapSpecification">Определяет, какие факты должны быть добавлены</param>
        /// <returns></returns>
        public ImportDocumentMetadataBuilder<TDto> ImportToFact<TFact>(
            Func<TDto, FindSpecification<TFact>> findSpecificationProvider,
            IMapSpecification<TDto, IReadOnlyCollection<TFact>> mapSpecification)
        {
            return this.WithFeatures(new ImportDocumentFeature<TDto, TFact>(findSpecificationProvider, mapSpecification));
        }
    }
}