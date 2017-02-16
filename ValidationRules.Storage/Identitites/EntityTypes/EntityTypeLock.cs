using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeLock : EntityTypeBase<EntityTypeLock>
    {
        public override int Id { get; } = EntityTypeIds.Lock;
        public override string Description { get; } = nameof(EntityTypeIds.Lock);
    }
}