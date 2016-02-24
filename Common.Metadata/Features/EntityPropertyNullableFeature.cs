using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.River.Common.Metadata.Features
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