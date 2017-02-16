using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeBill : EntityTypeBase<EntityTypeBill>
    {
        public override int Id { get; } = EntityTypeIds.Bill;
        public override string Description { get; } = nameof(EntityTypeIds.Bill);
    }
}