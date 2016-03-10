using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Elements
{
    public class ImportDocumentFeature<TDto, TFact> : IMetadataFeature
    {
        public ImportDocumentFeature(
            Func<TDto, FindSpecification<TFact>> findSpecificationProvider,
            IMapSpecification<TDto, IReadOnlyCollection<TFact>> mapSpecification)
        {
            FindSpecificationProvider = findSpecificationProvider;
            MapSpecification = mapSpecification;
        }

        public Func<TDto, FindSpecification<TFact>> FindSpecificationProvider { get; }

        public IMapSpecification<TDto, IReadOnlyCollection<TFact>> MapSpecification { get; }
    }
}