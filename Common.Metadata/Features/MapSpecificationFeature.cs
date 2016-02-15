using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata.Features
{
    public class MapSpecificationFeature<TSource, TTarget> : IMetadataFeature
    {
        public MapSpecificationFeature(MapSpecification<TSource, TTarget> mapSpecificationProvider)
        {
            MapSpecificationProvider = mapSpecificationProvider;
        }

        public MapSpecification<TSource, TTarget> MapSpecificationProvider { get; }
    }
}