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
        /// Add data import description from document to fact table.
        /// </summary>
        /// <typeparam name="TFact"></typeparam>
        /// <param name="findSpecificationProvider">Defines facts to be removed</param>
        /// <param name="mapSpecification">Defines facts to be created</param>
        /// <returns></returns>
        public ImportDocumentMetadataBuilder<TDto> ImportToFacts<TFact>(
            Func<TDto, FindSpecification<TFact>> findSpecificationProvider,
            IMapSpecification<TDto, IReadOnlyCollection<TFact>> mapSpecification)
        {
            return this.WithFeatures(new ImportDocumentFeature<TDto, TFact>(findSpecificationProvider, mapSpecification));
        }
    }
}