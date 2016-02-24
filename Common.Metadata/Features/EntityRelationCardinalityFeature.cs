using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.River.Common.Metadata.Elements;

namespace NuClear.River.Common.Metadata.Features
{
    public sealed class EntityRelationCardinalityFeature : IUniqueMetadataFeature
    {
        public EntityRelationCardinalityFeature(EntityRelationCardinality cardinality, EntityElement target)
        {
            Cardinality = cardinality;
            Target = target;
        }

        public EntityRelationCardinality Cardinality { get; private set; }

        public EntityElement Target { get; private set; }
    }
}