using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata;

namespace NuClear.ValidationRules.Domain.EntityTypes
{
    public sealed class EntityTypePeriod : EntityTypeBase<EntityTypePeriod>
    {
        public override int Id { get; } = EntityTypeIds.Period;
        public override string Description { get; } = nameof(EntityTypeIds.Period);
    }
}
