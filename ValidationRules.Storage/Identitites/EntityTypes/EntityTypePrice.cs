using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypePrice : EntityTypeBase<EntityTypePrice>
    {
        public override int Id { get; } = EntityTypeIds.Price;
        public override string Description { get; } = nameof(EntityTypeIds.Price);
    }
}
