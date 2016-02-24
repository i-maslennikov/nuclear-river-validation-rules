using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata;

namespace NuClear.ValidationRules.Domain.EntityTypes
{
    public sealed class EntityTypePrice : EntityTypeBase<EntityTypePrice>
    {
        public override int Id { get; } = EntityTypeIds.Price;
        public override string Description { get; } = nameof(EntityTypeIds.Price);
    }
}
