using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata;

namespace NuClear.ValidationRules.Domain.EntityTypes
{
    public sealed class EntityTypeOrder : EntityTypeBase<EntityTypeOrder>
    {
        public override int Id { get; } = EntityTypeIds.Order;
        public override string Description { get; } = nameof(EntityTypeIds.Order);
    }
}
