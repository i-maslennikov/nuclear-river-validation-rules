using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeOrder : EntityTypeBase<EntityTypeOrder>
    {
        public override int Id { get; } = EntityTypeIds.Order;
        public override string Description { get; } = nameof(EntityTypeIds.Order);
    }
}
