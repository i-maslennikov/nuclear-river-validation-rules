using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.Querying.Metadata.Features
{
    public sealed class EntityPropertyNullableFeature : IUniqueMetadataFeature
    {
        public EntityPropertyNullableFeature(bool isNullable)
        {
            IsNullable = isNullable;
        }

        public bool IsNullable { get; private set; }
    }
}