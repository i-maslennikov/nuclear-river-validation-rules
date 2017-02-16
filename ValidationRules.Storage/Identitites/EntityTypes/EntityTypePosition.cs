using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypePosition : EntityTypeBase<EntityTypePosition>
    {
        public override int Id { get; } = EntityTypeIds.Position;
        public override string Description { get; } = nameof(EntityTypeIds.Position);
    }
}
